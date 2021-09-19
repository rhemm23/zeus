using System.Text;

namespace Zeus.Tokens.Select {

  class SelectStatement : IWriteSql {

    private SelectQueryExpression _selectQueryExpression;

    public SelectStatement(SelectQueryExpression selectQueryExpression) {
      this._selectQueryExpression = selectQueryExpression;
    }

    public void WriteSql(StringBuilder sql) {
      this._selectQueryExpression.WriteSql(sql);
      sql.Append(";");
    }
  }
}
