using System.Data.SqlClient;
using Zeus.QueryBuilders;

namespace Zeus.Queries {

  class DeleteQuery<T> : Query<T> {

    private DeleteQueryBuilder _deleteQueryBuilder;

    public DeleteQuery(SqlConnection connection, T obj) : base(connection) {
      this._deleteQueryBuilder = new DeleteQueryBuilder(typeof(T), obj);
    }

    public bool Delete() {
      return this.ExecuteNonQuery() == 1;
    }

    public override SqlCommand GetSqlCommand() {
      return this._deleteQueryBuilder.GetSqlCommand();
    }
  }
}
