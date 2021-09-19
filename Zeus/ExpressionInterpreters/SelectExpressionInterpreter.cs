using System.Linq.Expressions;
using Zeus.QueryBuilders;
using Zeus.Tokens.Select;
using System.Reflection;
using System;

namespace Zeus {

  class SelectExpressionInterpreter<T> {

    private Expression<Func<T, object>> _selectExpression;
    private QueryBuilder _queryBuilder;

    public SelectExpressionInterpreter(QueryBuilder queryBuilder, Expression<Func<T, object>> selectExpression) {
      this._selectExpression = selectExpression;
      this._queryBuilder = queryBuilder;
    }

    public SelectItem GetSelectItem() {
      Expression currentExpression = this._selectExpression.Body;
      while (currentExpression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert) {
        currentExpression = unaryExpression.Operand;
      }
      if (currentExpression is MemberExpression memberExpression && memberExpression.Expression is ParameterExpression && memberExpression.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          return new SelectColumn(this._queryBuilder.GetTableAlias(typeof(T)), columnDefinition.Name);
        }
      }
      throw new InvalidSelectExpressionException(this._selectExpression);
    }
  }
}
