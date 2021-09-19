using System.Collections.Generic;
using System.Linq.Expressions;
using Zeus.Tokens.Predicates;
using Zeus.QueryBuilders;
using System.Reflection;
using Zeus.Exceptions;
using Zeus.Tokens;
using System;

namespace Zeus.ExpressionInterpreters {

  class ConditionExpressionInterpreter<T> {

    private Expression<Predicate<T>> _condition;
    private QueryBuilder _queryBuilder;

    public ConditionExpressionInterpreter(QueryBuilder queryBuilder, Expression<Predicate<T>> condition) {
      this._queryBuilder = queryBuilder;
      this._condition = condition;
    }

    private Tokens.Expression ParseConstantExpression(ConstantExpression constantExpression) {
      return this._queryBuilder.AddParameter(constantExpression.Value);
    }

    private Tokens.Expression ParseMemberAccessExpression(MemberExpression memberExpression) {
      if (memberExpression.Expression is System.Linq.Expressions.ParameterExpression && memberExpression.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          string tableAlias = this._queryBuilder.GetTableAlias(typeof(T));
          return new ColumnExpression(tableAlias, columnDefinition.Name);
        }
      } else {
        Stack<MemberInfo> memberInfos = new Stack<MemberInfo>();
        MemberExpression currentExpression = memberExpression;
        while (currentExpression.Expression is MemberExpression nextMemberExpression) {
          memberInfos.Push(currentExpression.Member);
          currentExpression = nextMemberExpression;
        }
        if (currentExpression.Expression is ConstantExpression constantExpression) {
          object currentValue = constantExpression.Value;
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
          }
          return this._queryBuilder.AddParameter(currentValue);
        }
      }
      throw new InvalidMemberAccessException();
    }

    private SearchConditionWithoutMatch ParseSearchConditionWithoutMatch(System.Linq.Expressions.Expression expression) {
      switch (expression) {
        case BinaryExpression binaryExpression:
          return this.ParseBinaryExpression(binaryExpression);

        default:
          throw new InvalidConditionExpressionException();
      }
    }

    private Tokens.Expression ParseExpression(System.Linq.Expressions.Expression expression) {
      switch (expression) {
        case MemberExpression memberExpression:
          return this.ParseMemberAccessExpression(memberExpression);

        case ConstantExpression constantExpression:
          return this.ParseConstantExpression(constantExpression);

        default:
          throw new InvalidConditionExpressionException();
      }
    }

    private bool IsExpressionConstantNull(System.Linq.Expressions.Expression expression) {
      switch (expression) {
        case ConstantExpression constantExpression:
          return constantExpression.Value == null;

        case MemberExpression memberExpression:
          Stack<MemberInfo> memberInfos = new Stack<MemberInfo>();
          while (memberExpression.Expression is MemberExpression nextMemberExpression) {
            memberInfos.Push(memberExpression.Member);
            memberExpression = nextMemberExpression;
          }
          if (memberExpression.Expression is ConstantExpression rootConstantExpression) {
            object currentValue = rootConstantExpression.Value;
            if (currentValue == null) {
              return true;
            }
            while(memberInfos.Count > 0) {
              switch (memberInfos.Pop()) {
                case FieldInfo fieldInfo:
                  currentValue = fieldInfo.GetValue(currentValue);
                  break;

                case PropertyInfo propertyInfo:
                  currentValue = propertyInfo.GetValue(currentValue);
                  break;

                default:
                  return false;
              }
              if (currentValue == null) {
                return true;
              }
            }
          }
          break;
      }
      return false;
    }

    private SearchConditionWithoutMatch ParseBinaryExpression(BinaryExpression binaryExpression) {
      switch (binaryExpression.NodeType) {
        case ExpressionType.And:
        case ExpressionType.AndAlso:

          SearchConditionWithoutMatch leftAndPredicate = this.ParseSearchConditionWithoutMatch(binaryExpression.Left);
          SearchConditionWithoutMatch rightAndPredicate = this.ParseSearchConditionWithoutMatch(binaryExpression.Right);

          return new AndSearchConditionWithoutMatch(leftAndPredicate, rightAndPredicate);

        case ExpressionType.Or:
        case ExpressionType.OrElse:

          SearchConditionWithoutMatch leftOrPredicate = this.ParseSearchConditionWithoutMatch(binaryExpression.Left);
          SearchConditionWithoutMatch rightOrPredicate = this.ParseSearchConditionWithoutMatch(binaryExpression.Right);

          return new OrSearchConditionWithoutMatch(leftOrPredicate, rightOrPredicate);

        default:

          bool isNullPredicate = false;
          Tokens.Expression nullPredicateExpression = null;

          if (this.IsExpressionConstantNull(binaryExpression.Left)) {
            isNullPredicate = true;
            nullPredicateExpression = this.ParseExpression(binaryExpression.Right);
          } else if (this.IsExpressionConstantNull(binaryExpression.Right)) {
            isNullPredicate = true;
            nullPredicateExpression = this.ParseExpression(binaryExpression.Left);
          }
          if (isNullPredicate) {
            switch (binaryExpression.NodeType) {
              case ExpressionType.Equal:
                return new SearchConditionWithoutMatch(
                  new IsNullPredicate(nullPredicateExpression)
                );

              case ExpressionType.NotEqual:
                return new SearchConditionWithoutMatch(
                  new IsNotNullPredicate(nullPredicateExpression)
                );

              default:
                throw new InvalidConditionExpressionException();
            }
          }

          Tokens.Expression leftExpression = this.ParseExpression(binaryExpression.Left);
          Tokens.Expression rightExpression = this.ParseExpression(binaryExpression.Right);

          return new SearchConditionWithoutMatch(
            new ComparisonPredicate(
              this.MapOperator(binaryExpression.NodeType),
              leftExpression,
              rightExpression
            )
          );
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
      SearchConditionWithoutMatch searchConditionWithoutMatch = this.ParseSearchConditionWithoutMatch(this._condition.Body);
      return new SearchCondition(searchConditionWithoutMatch);
    }
  }
}
