using System.Collections.Generic;
using System.Data.SqlClient;

namespace Zeus.Queries {

  public abstract class Query<T> {

    protected SqlConnection Connection { get; }

    public Query(SqlConnection connection) {
      this.Connection = connection;
    }

    public abstract SqlCommand GetSqlCommand();

    public virtual T First() {
      ObjectReader objectReader = new ObjectReader(this.GetDataReader(), typeof(T));
      T result = (T)objectReader.ReadObject();
      this.Connection.Close();
      return result;
    }

    public IEnumerable<T> All() {
      ObjectReader objectReader = new ObjectReader(this.GetDataReader(), typeof(T));
      foreach (object obj in objectReader.ReadAllObjects()) {
        yield return (T)obj;
      }
      this.Connection.Close();
    }

    protected SqlDataReader GetDataReader() {
      SqlCommand command = this.GetSqlCommand();
      command.Connection = this.Connection;
      return command.ExecuteReader();
    }
  }
}
