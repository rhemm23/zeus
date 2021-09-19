using System.Text;

namespace Zeus.Tokens {

  class TableSource : IWriteSql {

    private string _tableAlias;
    private string _tableName;

    public TableSource(string tableName) {
      this._tableName = tableName;
    }

    public TableSource(string tableName, string tableAlias) {
      this._tableAlias = tableAlias;
      this._tableName = tableName;
    }

    public void WriteSql(StringBuilder sql) {
      sql.Append(this._tableName);
      
      if (this._tableAlias != null) {
        sql.Append(" AS ");
        sql.Append(this._tableAlias);
      }
    }
  }
}
