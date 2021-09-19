using System.Text;

namespace Zeus.Tokens.Expressions {

  class ColumnExpression : Expression {

    private string _column;
    private string _source;

    public ColumnExpression(string source, string column) {
      this._column = column;
      this._source = source;
    }

    public override void WriteSql(StringBuilder sql) {
      sql.Append($"{this._source}.{this._column}");
    }
  }
}
