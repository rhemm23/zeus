using System.Data.SqlClient;
using Zeus.QueryBuilders;

namespace Zeus.Queries {

  public class UpdateQuery<T> : Query<T> {

    private UpdateQueryBuilder _updateQueryBuilder;

    public UpdateQuery(SqlConnection connection, T obj) : base(connection) {
      this._updateQueryBuilder = new UpdateQueryBuilder(typeof(T), obj);
    }

    public bool Update() {
      return this.ExecuteNonQuery() == 1;
    }

    public override SqlCommand GetSqlCommand() {
      return this._updateQueryBuilder.GetSqlCommand();
    }
  }
}
