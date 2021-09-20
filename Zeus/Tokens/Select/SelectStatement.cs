using System.Collections.Generic;
using System.Text;

namespace Zeus.Tokens.Select {

  class SelectStatement : IWriteSql {

    private SelectQueryExpression _selectQueryExpression;
    private List<OrderByClause> _orderByClauses;

    public SelectStatement(SelectQueryExpression selectQueryExpression) {
      this._selectQueryExpression = selectQueryExpression;
    }

    public SelectStatement(SelectQueryExpression selectQueryExpression, List<OrderByClause> orderByClauses) {
      this._selectQueryExpression = selectQueryExpression;
      this._orderByClauses = orderByClauses;
    }

    public void WriteSql(StringBuilder sql) {
      this._selectQueryExpression.WriteSql(sql);
      if (this._orderByClauses != null && this._orderByClauses.Count > 0) {
        sql.Append(" ORDER BY ");
        for (int i = 0; i < this._orderByClauses.Count; i++) {
          if (i > 0) {
            sql.Append(", ");
          }
          this._orderByClauses[i].WriteSql(sql);
        }
      }
      sql.Append(";");
    }
  }
}
