using Zeus.Tokens.SearchConditions;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zeus.Tokens.Predicates;
using Zeus.QueryBuilders;
using System.Reflection;
using Zeus.Exceptions;
using System;

namespace Zeus.ExpressionInterpreters {

  class ConditionExpressionInterpreter<T> {

    private class ExpressionDescriptor {
    
      public enum DescriptorType {
        Expression,
        Condition,
        Null
      }

      public Tokens.Expressions.Expression Expression { get; }

      public SearchConditionWithoutMatch Condition { get; }

      public bool IsBooleanAccess { get; }

      public DescriptorType Type { get; }

      public ExpressionDescriptor(Tokens.Expressions.Expression expression, bool isBooleanAccess) {
        this.IsBooleanAccess = isBooleanAccess;
        this.Type = DescriptorType.Expression;
        this.Expression = expression;
      }

      public ExpressionDescriptor(SearchConditionWithoutMatch condition) {
        this.Type = DescriptorType.Condition;
        this.Condition = condition;
      }

      public ExpressionDescriptor() {
        this.Type = DescriptorType.Null;
      }
    }

    private Expression<Predicate<T>> _condition;
    private QueryBuilder _queryBuilder;

    public ConditionExpressionInterpreter(QueryBuilder queryBuilder, Expression<Predicate<T>> condition) {
      this._queryBuilder = queryBuilder;
      this._condition = condition;
    }

    private ExpressionDescriptor ParseConstantExpression(ConstantExpression constantExpression) {
      if (constantExpression.Value == null) {
        return new ExpressionDescriptor();
      } else {
        return new ExpressionDescriptor(this._queryBuilder.AddParameter(constantExpression.Value), false);
      }
    }

    private ExpressionDescriptor ParseMemberAccessExpression(MemberExpression memberExpression) {
      if (memberExpression.Expression is ParameterExpression && memberExpression.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          string tableAlias = this._queryBuilder.GetTableAlias(typeof(T));
          return new ExpressionDescriptor(
            new Tokens.Expressions.ColumnExpression(tableAlias, columnDefinition.Name),
            propertyInfo.PropertyType == typeof(bool)
          );
        }
      } else {
        MemberExpression currentExpression = memberExpression;
        Stack<MemberInfo> memberInfos = new Stack<MemberInfo>();
        memberInfos.Push(currentExpression.Member);
        while (currentExpression.Expression is MemberExpression nextMemberExpression) {
          memberInfos.Push(nextMemberExpression.Member);
          currentExpression = nextMemberExpression;
        }
        if (currentExpression.Expression is ConstantExpression constantExpression) {
          object currentValue = constantExpression.Value;
          if (currentValue == null) {
            return new ExpressionDescriptor();
          }
          while (memberInfos.Count > 0) {
            MemberInfo memberInfo = memberInfos.Pop();
            switch (memberInfo) {
              case FieldInfo fieldInfo:
                currentValue = fieldInfo.GetValue(currentValue);
                break;

              case PropertyInfo objectPropertyInfo:
                currentValue = objectPropertyInfo.GetValue(currentValue);
                break;

              default:
                throw new InvalidMemberAccessException();
            }
            if (currentValue == null) {
              return new ExpressionDescriptor();
            }
          }
          return new ExpressionDescriptor(this._queryBuilder.AddParameter(currentValue), false);
        }
      }
      throw new InvalidMemberAccessException();
    }

    private ExpressionDescriptor ParseExpression(Expression expression) {
      switch (expression) {
        case BinaryExpression binaryExpression:
          return this.ParseBinaryExpression(binaryExpression);

        case MemberExpression memberExpression:
          return this.ParseMemberAccessExpression(memberExpression);

        case ConstantExpression constantExpression:
          return this.ParseConstantExpression(constantExpression);

        default:
          throw new InvalidConditionExpressionException();
      }
    }

    private SearchConditionWithoutMatch ConvertExpressionDescriptorToCondition(ExpressionDescriptor expressionDescriptor) {
      switch (expressionDescriptor.Type) {
        case ExpressionDescriptor.DescriptorType.Condition:
          return expressionDescriptor.Condition;

        case ExpressionDescriptor.DescriptorType.Expression:
          if (expressionDescriptor.IsBooleanAccess) {
            return new SearchConditionWithoutMatch(
              new ComparisonPredicate(
                ComparisonPredicate.Operator.Equal,
                expressionDescriptor.Expression,
                this._queryBuilder.AddParameter(true)
              )
            );
          } else {
            throw new InvalidConditionExpressionException();
          }

        default:
          throw new InvalidConditionExpressionException();
      }
    }

    private ExpressionDescriptor ParseBinaryExpression(BinaryExpression binaryExpression) {
      switch (binaryExpression.NodeType) {
        case ExpressionType.And:
        case ExpressionType.AndAlso:

          ExpressionDescriptor leftAndPredicate = this.ParseExpression(binaryExpression.Left);
          ExpressionDescriptor rightAndPredicate = this.ParseExpression(binaryExpression.Right);

          SearchConditionWithoutMatch leftAndCondition = this.ConvertExpressionDescriptorToCondition(leftAndPredicate);
          SearchConditionWithoutMatch rightAndCondition = this.ConvertExpressionDescriptorToCondition(rightAndPredicate);

          return new ExpressionDescriptor(new AndSearchConditionWithoutMatch(leftAndCondition, rightAndCondition));

        case ExpressionType.Or:
        case ExpressionType.OrElse:

          ExpressionDescriptor leftOrPredicate = this.ParseExpression(binaryExpression.Left);
          ExpressionDescriptor rightOrPredicate = this.ParseExpression(binaryExpression.Right);

          SearchConditionWithoutMatch leftOrCondition = this.ConvertExpressionDescriptorToCondition(leftOrPredicate);
          SearchConditionWithoutMatch rightOrCondition = this.ConvertExpressionDescriptorToCondition(rightOrPredicate);

          return new ExpressionDescriptor(new OrSearchConditionWithoutMatch(leftOrCondition, rightOrCondition));

        default:

          ExpressionDescriptor leftExpression = this.ParseExpression(binaryExpression.Left);
          ExpressionDescriptor rightExpression = this.ParseExpression(binaryExpression.Right);

          Predicate predicate;

          if (leftExpression.Type == ExpressionDescriptor.DescriptorType.Expression && rightExpression.Type == ExpressionDescriptor.DescriptorType.Expression) {
            predicate = new ComparisonPredicate(
              this.MapOperator(binaryExpression.NodeType),
              leftExpression.Expression,
              rightExpression.Expression
            );
          } else if (leftExpression.Type == ExpressionDescriptor.DescriptorType.Null && rightExpression.Type == ExpressionDescriptor.DescriptorType.Expression) {
            switch (binaryExpression.NodeType) {
              case ExpressionType.Equal:
                predicate = new IsNullPredicate(rightExpression.Expression);
                break;

              case ExpressionType.NotEqual:
                predicate = new IsNotNullPredicate(rightExpression.Expression);
                break;

              default:
                throw new InvalidBinaryOperatorException();
            }
          } else if (leftExpression.Type == ExpressionDescriptor.DescriptorType.Expression && rightExpression.Type == ExpressionDescriptor.DescriptorType.Null) {
            switch (binaryExpression.NodeType) {
              case ExpressionType.Equal:
                predicate = new IsNullPredicate(leftExpression.Expression);
                break;

              case ExpressionType.NotEqual:
                predicate = new IsNotNullPredicate(leftExpression.Expression);
                break;

              default:
                throw new InvalidBinaryOperatorException();
            }
          } else {
            throw new InvalidBinaryOperatorException();
          }
          return new ExpressionDescriptor(new SearchConditionWithoutMatch(predicate));
      }
    }

    private ComparisonPredicate.Operator MapOperator(ExpressionType expressionType) {
      switch (expressionType) {
        case ExpressionType.Equal:
          return ComparisonPredicate.Operator.Equal;

        case ExpressionType.NotEqual:
          return ComparisonPredicate.Operator.NotEqual;

        case ExpressionType.GreaterThan:
          return ComparisonPredicate.Operator.GreaterThan;

        case ExpressionType.GreaterThanOrEqual:
          return ComparisonPredicate.Operator.GreaterThanOrEqual;

        case ExpressionType.LessThan:
          return ComparisonPredicate.Operator.LessThan;

        case ExpressionType.LessThanOrEqual:
          return ComparisonPredicate.Operator.LessThanOrEqual;

        default:
          throw new InvalidBinaryOperatorException();
      }
    }

    public SearchCondition GetSearchCondition() {
      ExpressionDescriptor expressionDescriptor = this.ParseExpression(this._condition.Body);
      return new SearchCondition(this.ConvertExpressionDescriptorToCondition(expressionDescriptor));
    }
  }
}
