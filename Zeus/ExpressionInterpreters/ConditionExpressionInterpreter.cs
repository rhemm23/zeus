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

    private Stack<Tokens.Expression> _expressions;
    private Expression<Predicate<T>> _condition;
    private QueryBuilder _queryBuilder;

    public ConditionExpressionInterpreter(QueryBuilder queryBuilder, Expression<Predicate<T>> condition) {
      this._expressions = new Stack<Tokens.Expression>();
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

    private Predicate ParsePredicate(System.Linq.Expressions.Expression expression) {
      
    }

    private Tokens.Expression ParseExpression(System.Linq.Expressions.Expression expression) {

    }

    private SearchConditionWithoutMatch ParseBinaryExpression(BinaryExpression binaryExpression) {
      switch (binaryExpression.NodeType) {
        case ExpressionType.And:
        case ExpressionType.AndAlso:

          Predicate leftAndPredicate = this.ParsePredicate(binaryExpression.Left);
          Predicate rightAndPredicate = this.ParsePredicate(binaryExpression.Right);

          return new AndSearchConditionWithoutMatch(leftAndPredicate, rightAndPredicate);

        case ExpressionType.Or:
        case ExpressionType.OrElse:

          Predicate leftOrPredicate = this.ParsePredicate(binaryExpression.Left);
          Predicate rightOrPredicate = this.ParsePredicate(binaryExpression.Right);

          return new OrSearchConditionWithoutMatch(leftOrPredicate, rightOrPredicate);

        default:

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

    protected override System.Linq.Expressions.Expression VisitBinary(BinaryExpression node) {

      this.Visit(node.Left);
      this.Visit(node.Right);

      switch (node.NodeType) {
        case ExpressionType.Equal:
          this._expressions.Push(new ComparisonPredicate(Com))

      }
    }

    public SearchCondition GetSearchCondition() {

      switch (this._condition.Body) {

        case ConstantExpression constantExpression:

          break;

      }


      this.Visit(this._condition);
      throw new NotImplementedException();
    }
  }
}
