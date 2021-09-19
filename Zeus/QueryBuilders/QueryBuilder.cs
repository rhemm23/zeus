using System.Collections.Generic;
using Zeus.Tokens.Expressions;
using System.Data.SqlClient;
using System;

namespace Zeus.QueryBuilders {

  abstract class QueryBuilder {

    public Type PrimaryTableType { get; }

    private Dictionary<string, object> _parameters;
    private Dictionary<Type, string> _tableAliases;
    private int _parameterCount;
    private int _tableCount;

    public QueryBuilder(Type primaryTableType) {
      this._tableAliases = new Dictionary<Type, string>();
      this._parameters = new Dictionary<string, object>();

      this.PrimaryTableType = primaryTableType;
      this._parameterCount = 0;
      this._tableCount = 0;
    }

    public abstract string GetSql();

    public SqlCommand GetSqlCommand() {
      SqlCommand command = new SqlCommand(this.GetSql());
      foreach (KeyValuePair<string, object> parameter in this._parameters) {
        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
      }
      return command;
    }

    public ParameterExpression AddParameter(object value) {
      string parameterName = this.GetParameterName();
      this._parameters.Add(parameterName, value);
      return new ParameterExpression(parameterName);
    }

    private string GetParameterName() {
      return $"p_{this._parameterCount++}";
    }

    public string GetTableAlias(Type type) {
      if (this._tableAliases.TryGetValue(type, out string alias)) {
        return alias;
      } else {
        string newAlias = $"t_{this._tableCount++ :00}";
        this._tableAliases.Add(type, newAlias);
        return newAlias;
      }
    }
  }
}
