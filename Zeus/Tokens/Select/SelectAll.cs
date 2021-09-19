using System.Text;

namespace Zeus.Tokens.Select {

  class SelectAll : SelectItem {

    private string _scope;

    public SelectAll(string scope) {
      this._scope = scope;
    }

    public override void WriteSql(StringBuilder sql) {
      sql.Append($"{this._scope}.*");
    }
  }
}
