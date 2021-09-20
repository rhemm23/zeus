using System.Linq.Expressions;
using Zeus.QueryBuilders;
using System.Reflection;
using System;

namespace Zeus {

  class ColumnAccessExpressionInterpreter<T> {

    private Expression<Func<T, object>> _selectExpression;
    private QueryBuilder _queryBuilder;

    public ColumnAccessExpressionInterpreter(QueryBuilder queryBuilder, Expression<Func<T, object>> selectExpression) {
      this._selectExpression = selectExpression;
      this._queryBuilder = queryBuilder;
    }

    public ColumnDefinition GetAccessedColumn() {
      Expression currentExpression = this._selectExpression.Body;
      while (currentExpression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert) {
        currentExpression = unaryExpression.Operand;
      }
      if (currentExpression is MemberExpression memberExpression && memberExpression.Expression is ParameterExpression && memberExpression.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          return columnDefinition;
        }
      }
      throw new InvalidSelectExpressionException(this._selectExpression);
    }
  }
}
