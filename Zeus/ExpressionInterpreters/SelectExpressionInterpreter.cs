using System.Linq.Expressions;
using System.Reflection;
using System;

namespace Zeus {

  class SelectExpressionInterpreter<T> : ExpressionVisitor {

    private Expression<Func<T, object>> _selectExpression;
    private string _columnName;

    public SelectExpressionInterpreter(Expression<Func<T, object>> selectExpression) {
      this._selectExpression = selectExpression;
    }

    protected override Expression VisitMember(MemberExpression node) {
      if (node.Member.DeclaringType == typeof(T) && node.Member is PropertyInfo propertyInfo) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        if (tableDefinition.ColumnDefinitionsByPropertyInfo.TryGetValue(propertyInfo, out ColumnDefinition columnDefinition)) {
          this._columnName = columnDefinition.Name;
        }
      }
      return base.VisitMember(node);
    }

    public string GetColumnName() {
      this.Visit(this._selectExpression);
      if (this._columnName == null) {
        throw new InvalidSelectExpressionException(this._selectExpression);
      } else {
        return this._columnName;
      }
    }
  }
}
