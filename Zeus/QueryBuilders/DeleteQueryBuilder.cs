using System.Text;
using System;

namespace Zeus.QueryBuilders {

  class DeleteQueryBuilder : QueryBuilder {

    private object _object;

    public DeleteQueryBuilder(Type primaryTableType, object obj) : base(primaryTableType) {
      this._object = obj;
    }

    public override string GetSql() {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(this.PrimaryTableType);
      StringBuilder sql = new StringBuilder();

      sql.Append($"DELETE {tableDefinition.Name} WHERE ");
      sql.Append(tableDefinition.PrimaryKey.Name);
      sql.Append(" = ");

      this.AddParameter(tableDefinition.PrimaryKey.PropertyInfo.GetValue(this._object)).WriteSql(sql);
      sql.Append(";");

      return sql.ToString();
    }
  }
}
