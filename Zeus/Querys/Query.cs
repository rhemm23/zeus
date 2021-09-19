using System.Collections.Generic;
using System.Data.SqlClient;

namespace Zeus.Querys {

  public abstract class Query<T> {

    private SqlConnection _connection;

    public Query(SqlConnection connection) {
      this._connection = connection;
    }

    public abstract SqlCommand GetSqlCommand();

    public virtual T First() {
      ObjectReader objectReader = new ObjectReader(this.GetDataReader(), typeof(T));
      return (T)objectReader.ReadObject();
    }

    public IEnumerable<T> All() {
      ObjectReader objectReader = new ObjectReader(this.GetDataReader(), typeof(T));
      foreach (object obj in objectReader.ReadAllObjects()) {
        yield return (T)obj;
      }
      this._connection.Close();
    }

    private SqlDataReader GetDataReader() {
      SqlCommand command = this.GetSqlCommand();
      command.Connection = this._connection;
      return command.ExecuteReader();
    }
  }
}
