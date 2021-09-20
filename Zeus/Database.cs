using System.Linq.Expressions;
using System.Data.SqlClient;
using Zeus.Queries;
using System;

namespace Zeus {

  public sealed class Database {

    private string _connectionString;

    public Database(string connectionString) {
      this._connectionString = connectionString;
    }

    public bool Delete<T>(T obj) {
      DeleteQuery<T> deleteQuery = new DeleteQuery<T>(this.GetNewConnection(), obj);
      return deleteQuery.Delete();
    }

    public bool Save<T>(T obj) {
      if (this.DoesPrimaryKeyHaveValue<T>(obj)) {
        UpdateQuery<T> updateQuery = new UpdateQuery<T>(this.GetNewConnection(), obj);
        return updateQuery.Update();
      } else {
        InsertQuery<T> insertQuery = new InsertQuery<T>(this.GetNewConnection(), obj);
        return insertQuery.Insert();
      }
    }

    public SelectQuery<T> Select<T>(params Expression<Func<T, object>>[] selectExpressions) {
      return new SelectQuery<T>(this.GetNewConnection(), selectExpressions);
    }

    private SqlConnection GetNewConnection() {
      SqlConnection connection = new SqlConnection(this._connectionString);
      connection.Open();
      return connection;
    }

    private bool DoesPrimaryKeyHaveValue<T>(object obj) {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
      object primaryKey = tableDefinition.PrimaryKey.PropertyInfo.GetValue(obj);

      switch (primaryKey) {
        case sbyte sb:
          return sb > 0;

        case byte b:
          return b > 0;

        case ushort us:
          return us > 0;

        case short s:
          return s > 0;

        case uint ui:
          return ui > 0;

        case int i:
          return i > 0;

        case ulong ul:
          return ul > 0;

        case long l:
          return l > 0;

        default:
          return primaryKey != null;
      }
    }
  }
}
