using Zeus.ExpressionInterpreters.ExpressionTranslations;
using Zeus.Tokens.SearchConditions;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zeus.Tokens.Predicates;
using Zeus.QueryBuilders;
using System.Reflection;
using Zeus.Exceptions;
using System.Data;
using System;

namespace Zeus.ExpressionInterpreters {

  class ConditionExpressionInterpreter<T> {

    private Expression<Predicate<T>> _condition;
    private QueryBuilder _queryBuilder;

    public ConditionExpressionInterpreter(QueryBuilder queryBuilder, Expression<Predicate<T>> condition) {
      this._queryBuilder = queryBuilder;
      this._condition = condition;
    }

    private ExpressionTranslation TranslateConstantExpression(ConstantExpression constantExpression) {
      if (constantExpression.Value == null) {
        return new NullExpressionTranslation();
      } else {
        return new ConstantExpressionTranslation(constantExpression.Value);
      }
    }

    private ExpressionTranslation TranslateMemberAccessExpression(MemberExpression memberExpression) {
      if (memberExpression.Expression is ParameterExpression && memberExpression.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          return new ColumnAccessExpressionTranslation(columnDefinition);
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
            return new NullExpressionTranslation();
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
              return new NullExpressionTranslation();
            }
          }
          return new ConstantExpressionTranslation(currentValue);
        }
      }
      throw new InvalidMemberAccessException();
    }

    private ExpressionTranslation TranslateExpression(Expression expression) {
      switch (expression) {
        case BinaryExpression binaryExpression:
          return this.ParseBinaryExpression(binaryExpression);

        case MemberExpression memberExpression:
          return this.TranslateMemberAccessExpression(memberExpression);

        case ConstantExpression constantExpression:
          return this.TranslateConstantExpression(constantExpression);

        default:
          throw new InvalidConditionExpressionException();
      }
    }

    private Tokens.Expressions.ColumnExpression GetColumnExpressionFromTranslation(ColumnAccessExpressionTranslation columnAccessExpressionTranslation) {
      return new Tokens.Expressions.ColumnExpression(
        this._queryBuilder.GetTableAlias(typeof(T)),
        columnAccessExpressionTranslation.ColumnDefinition.Name
      );
    }

    private SearchConditionWithoutMatch ConvertExpressionTranslationToCondition(ExpressionTranslation expressionDescriptor) {
      switch (expressionDescriptor) {
        case ConditionExpressionTranslation conditionExpressionTranslation:
          return conditionExpressionTranslation.Condition;

        case ColumnAccessExpressionTranslation columnAccessExpressionTranslation:
          if (columnAccessExpressionTranslation.ColumnDefinition.DbType == SqlDbType.Bit) {
            return new SearchConditionWithoutMatch(
              new ComparisonPredicate(
                ComparisonPredicate.Operator.Equal,
                new Tokens.Expressions.ColumnExpression(
                  this._queryBuilder.GetTableAlias(columnAccessExpressionTranslation.ColumnDefinition.PropertyInfo.DeclaringType),
                  columnAccessExpressionTranslation.ColumnDefinition.Name
                ),
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

    private ExpressionTranslation ParseBinaryExpression(BinaryExpression binaryExpression) {
      switch (binaryExpression.NodeType) {
        case ExpressionType.And:
        case ExpressionType.AndAlso:

          ExpressionTranslation leftAndPredicate = this.TranslateExpression(binaryExpression.Left);
          ExpressionTranslation rightAndPredicate = this.TranslateExpression(binaryExpression.Right);

          SearchConditionWithoutMatch leftAndCondition = this.ConvertExpressionTranslationToCondition(leftAndPredicate);
          SearchConditionWithoutMatch rightAndCondition = this.ConvertExpressionTranslationToCondition(rightAndPredicate);

          return new ConditionExpressionTranslation(new AndSearchConditionWithoutMatch(leftAndCondition, rightAndCondition));

        case ExpressionType.Or:
        case ExpressionType.OrElse:

          ExpressionTranslation leftOrPredicate = this.TranslateExpression(binaryExpression.Left);
          ExpressionTranslation rightOrPredicate = this.TranslateExpression(binaryExpression.Right);

          SearchConditionWithoutMatch leftOrCondition = this.ConvertExpressionTranslationToCondition(leftOrPredicate);
          SearchConditionWithoutMatch rightOrCondition = this.ConvertExpressionTranslationToCondition(rightOrPredicate);

          return new ConditionExpressionTranslation(new OrSearchConditionWithoutMatch(leftOrCondition, rightOrCondition));

        default:

          ExpressionTranslation leftExpression = this.TranslateExpression(binaryExpression.Left);
          ExpressionTranslation rightExpression = this.TranslateExpression(binaryExpression.Right);

          switch (leftExpression) {
            case ColumnAccessExpressionTranslation leftColumnAccessExpressionTranslation:
              switch (rightExpression) {
                case ConstantExpressionTranslation rightConstantExpressionTranslation:
                  return new ConditionExpressionTranslation(
                    new SearchConditionWithoutMatch(
                      new ComparisonPredicate(
                        this.MapOperator(binaryExpression.NodeType),
                        this.GetColumnExpressionFromTranslation(leftColumnAccessExpressionTranslation),
                        this._queryBuilder.AddParameter(rightConstantExpressionTranslation.Value)
                      )
                    )
                  );

                case ColumnAccessExpressionTranslation rightColumnAccessExpressionTranslation:
                  return new ConditionExpressionTranslation(
                    new SearchConditionWithoutMatch(
                      new ComparisonPredicate(
                        this.MapOperator(binaryExpression.NodeType),
                        this.GetColumnExpressionFromTranslation(leftColumnAccessExpressionTranslation),
                        this.GetColumnExpressionFromTranslation(rightColumnAccessExpressionTranslation)
                      )
                    )
                  );

                case NullExpressionTranslation _:
                  switch (binaryExpression.NodeType) {
                    case ExpressionType.Equal:
                      return new ConditionExpressionTranslation(
                        new SearchConditionWithoutMatch(
                          new IsNullPredicate(this.GetColumnExpressionFromTranslation(leftColumnAccessExpressionTranslation))
                        )
                      );

                    case ExpressionType.NotEqual:
                      return new ConditionExpressionTranslation(
                        new SearchConditionWithoutMatch(
                          new IsNotNullPredicate(this.GetColumnExpressionFromTranslation(leftColumnAccessExpressionTranslation))
                        )
                      );

                    default:
                      throw new InvalidConditionExpressionException();
                  }

                default:
                  throw new InvalidConditionExpressionException();
              }

            case ConstantExpressionTranslation leftConstantExpressionTranslation:
              switch (rightExpression) {
                case ColumnAccessExpressionTranslation rightColumnAccessExpressionTranslation:
                  return new ConditionExpressionTranslation(
                    new SearchConditionWithoutMatch(
                      new ComparisonPredicate(
                        this.MapOperator(binaryExpression.NodeType),
                        this.GetColumnExpressionFromTranslation(rightColumnAccessExpressionTranslation),
                        this._queryBuilder.AddParameter(leftConstantExpressionTranslation.Value)
                      )
                    )
                  );

                default:
                  throw new InvalidConditionExpressionException();
              }

            case NullExpressionTranslation _:
              switch (rightExpression) {
                case ColumnAccessExpressionTranslation rightColumnAccessExpressionTranslation:
                  switch (binaryExpression.NodeType) {
                    case ExpressionType.Equal:
                      return new ConditionExpressionTranslation(
                        new SearchConditionWithoutMatch(
                          new IsNullPredicate(this.GetColumnExpressionFromTranslation(rightColumnAccessExpressionTranslation))
                        )
                      );

                    case ExpressionType.NotEqual:
                      return new ConditionExpressionTranslation(
                        new SearchConditionWithoutMatch(
                          new IsNotNullPredicate(this.GetColumnExpressionFromTranslation(rightColumnAccessExpressionTranslation))
                        )
                      );

                    default:
                      throw new InvalidConditionExpressionException();
                  }

                default:
                  throw new InvalidConditionExpressionException();
              }

            default:
              throw new InvalidConditionExpressionException();
          }
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
      ExpressionTranslation expressionTranslation = this.TranslateExpression(this._condition.Body);
      return new SearchCondition(this.ConvertExpressionTranslationToCondition(expressionTranslation));
    }
  }
}
