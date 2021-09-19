using System.Text;

namespace Zeus.Tokens.Select {

  class SelectColumn : SelectItem {

    private string _columnName;
    private string _scope;

    public SelectColumn(string scope, string columnName) {
      this._columnName = columnName;
      this._scope = scope;
    }

    public override void WriteSql(StringBuilder sql) {
      sql.Append($"{this._scope}.{this._columnName}");
    }
  }
}
