using System.Data.SqlClient;
using Zeus.QueryBuilders;

namespace Zeus.Queries {

  public class InsertQuery<T> : Query<T> {

    private InsertQueryBuilder _insertQueryBuilder;
    private object _object;

    public InsertQuery(SqlConnection connection, object obj) : base(connection) {
      this._insertQueryBuilder = new InsertQueryBuilder(typeof(T), obj);
      this._object = obj;
    }

    public bool Insert() {
      SqlDataReader reader = this.GetDataReader();
      if (reader.Read()) {
        TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
        tableDefinition.PrimaryKey.PropertyInfo.SetValue(this._object, reader.GetValue(0));
        return true;
      } else {
        return false;
      }
    }

    public override SqlCommand GetSqlCommand() {
      return this._insertQueryBuilder.GetSqlCommand();
    }
  }
}
