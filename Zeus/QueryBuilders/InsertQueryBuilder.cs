using Zeus.Tokens.Expressions;
using System.Text;
using System;
using System.Collections.Generic;

namespace Zeus.QueryBuilders {

  class InsertQueryBuilder : QueryBuilder {

    private object _object;

    public InsertQueryBuilder(Type primaryTableType, object obj) : base(primaryTableType) {
      this._object = obj;
    }

    public override string GetSql() {

      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(this.PrimaryTableType);
      StringBuilder sql = new StringBuilder();

      sql.Append($"INSERT INTO {tableDefinition.Name} (");

      List<object> values = new List<object>();
      foreach (ColumnDefinition columnDefinition in tableDefinition.ColumnDefinitions) {
        object value = columnDefinition.PropertyInfo.GetValue(this._object);
        if (value != null && !columnDefinition.IsPrimaryKey) {
          if (values.Count > 0) {
            sql.Append(", ");
          }
          values.Add(value);
          sql.Append(columnDefinition.Name);
        }
      }

      sql.Append(") VALUES (");

      for (int i = 0; i < values.Count; i++) {
        if (i > 0) {
          sql.Append(", ");
        }
        ParameterExpression parameterExpression = this.AddParameter(values[i]);
        parameterExpression.WriteSql(sql);
      }

      sql.Append("); SELECT CAST(SCOPE_IDENTITY() AS INT);");
      return sql.ToString();
    }
  }
}
