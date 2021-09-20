using Zeus.Tokens.Expressions;
using System.Text;

namespace Zeus.Tokens {

  class OrderByClause : IWriteSql {

    private Expression _orderExpression;
    private bool _isAscending;

    public OrderByClause(Expression orderExpression, bool isAscending) {
      this._orderExpression = orderExpression;
      this._isAscending = isAscending;
    }

    public void WriteSql(StringBuilder sql) {
      this._orderExpression.WriteSql(sql);
      if (this._isAscending) {
        sql.Append(" ASC");
      } else {
        sql.Append(" DESC");
      }
    }
  }
}
