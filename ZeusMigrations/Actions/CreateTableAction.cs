using System.Collections.Generic;
using System.Data.SqlClient;
using ZeusMigrations.Models;
using System.Text;

namespace ZeusMigrations.Actions {

  public class CreateTableAction : IAction {

    private List<ColumnDefinition> _columns;
    private string _schemaName;
    private string _tableName;

    public CreateTableAction(string tableName, List<ColumnDefinition> columns) {
      this._tableName = tableName;
      this._columns = columns;
    }

    public CreateTableAction(string schemaName, string tableName, List<ColumnDefinition> columns) {
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
      sql.Append(" (\n");
      for (int i = 0; i < this._columns.Count; i++) {
        sql.Append($"  {this._columns[i]}");
        if (i != this._columns.Count - 1) {
          sql.Append(",");
        }
        sql.Append("\n");
      }
      sql.Append(");");
      using (SqlCommand command = new SqlCommand(sql.ToString(), connection)) {
        command.ExecuteNonQuery();
      }
    }

    public void Down(SqlConnection connection) {
      string query = $"DROP TABLE {this._tableName};";
      using (SqlCommand command = new SqlCommand(query, connection)) {
        command.ExecuteNonQuery();
      }
    }
  }
}
