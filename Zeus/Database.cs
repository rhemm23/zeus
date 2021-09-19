using System.Linq.Expressions;
using System.Data.SqlClient;
using Zeus.Querys;
using System;

namespace Zeus {

  public sealed class Database {

    private string _connectionString;

    public Database(string connectionString) {
      this._connectionString = connectionString;
    }

    public SelectQuery<T> Select<T>(params Expression<Func<T, object>>[] selectExpressions) {
      return new SelectQuery<T>(this.GetNewConnection(), selectExpressions);
    }

    private SqlConnection GetNewConnection() {
      SqlConnection connection = new SqlConnection(this._connectionString);
      connection.Open();
      return connection;
    }
  }
}
