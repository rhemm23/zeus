using System.Collections.Generic;
using System.Data.SqlClient;
using ZeusMigrations.Models;
using System.Text;

namespace ZeusMigrations.Actions {

  public class CreateTableAction : IAction {

    private IEnumerable<ColumnDefinition> _columns;
    private string _schemaName;
    private string _tableName;

    public CreateTableAction(string tableName, IEnumerable<ColumnDefinition> columns) {
      this._tableName = tableName;
      this._columns = columns;
    }

    public CreateTableAction(string schemaName, string tableName, IEnumerable<ColumnDefinition> columns) {
      this._schemaName = schemaName;
      this._tableName = tableName;
      this._columns = columns;
    }

    public void Up(SqlConnection connection) {
      StringBuilder sql = new StringBuilder("CREATE TABLE ");
      if (this._schemaName == null) {
        sql.Append(this._tableName);
      } else {
        sql.Append($"{this._schemaName}.{this._tableName}");
      }

    }

    public void Down(SqlConnection connection) {

    }
  }
}
