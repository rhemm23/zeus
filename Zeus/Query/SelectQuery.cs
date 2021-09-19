using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Zeus {

  public class SelectQuery<T> : IWriteSql {

    private IEnumerable<string> _columns;
    private SqlConnection _connection;

    public SelectQuery(SqlConnection connection, IEnumerable<string> columns) {
      this._connection = connection;
      this._columns = columns;
    }

    public IEnumerable<T> All() {
      ObjectReader objectReader = new ObjectReader(this.GetDataReader(), typeof(T));
      foreach (object obj in objectReader.ReadAllObjects()) {
        yield return (T)obj;
      }
      this._connection.Close();
    }

    private SqlDataReader GetDataReader() {
      SqlCommand command = new SqlCommand(this.GetQuerySql(), this._connection);
      return command.ExecuteReader();
    }

    private string GetQuerySql() {
      StringBuilder sb = new StringBuilder();
      this.WriteSql(sb);
      return sb.ToString();
    }

    public void WriteSql(StringBuilder sb) {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));

      sb.Append("SELECT ");

      if (this._columns.Count() > 0) {
        bool first = true;
        foreach (string column in this._columns) {
          if (!first) {
            sb.Append(", ");
          } else {
            first = false;
          }
          sb.Append(column);
        }
      } else {
        sb.Append("*");
      }
      sb.Append($" FROM {tableDefinition.Name};");
    }
  }
}
