using System.Text;
using System;

namespace Zeus.QueryBuilders {

  class UpdateQueryBuilder : QueryBuilder {

    private object _object;

    public UpdateQueryBuilder(Type primaryTableType, object obj) : base(primaryTableType) {
      this._object = obj;
    }

    public override string GetSql() {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(this.PrimaryTableType);
      StringBuilder sql = new StringBuilder();

      sql.Append($"UPDATE {tableDefinition.Name} SET ");
      int setCount = 0;

      foreach (ColumnDefinition columnDefinition in tableDefinition.ColumnDefinitions) {
        object value = columnDefinition.PropertyInfo.GetValue(this._object);
        if (value != null && !columnDefinition.IsPrimaryKey) {
          if (setCount++ > 0) {
            sql.Append(", ");
          }
          sql.Append($"{columnDefinition.Name} = ");
          this.AddParameter(value).WriteSql(sql);
        }
      }

      sql.Append($" WHERE {tableDefinition.PrimaryKey.Name} = ");
      this.AddParameter(tableDefinition.PrimaryKey.PropertyInfo.GetValue(this._object)).WriteSql(sql);
      sql.Append(";");

      return sql.ToString();
    }
  }
}
