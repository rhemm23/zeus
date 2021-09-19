﻿using System.Collections.Generic;
using Zeus.Tokens;
using System;

namespace Zeus.QueryBuilders {

  abstract class QueryBuilder {

    private Dictionary<string, object> _parameters;
    private Dictionary<Type, string> _tableAliases;
    private int _parameterCount;
    private int _tableCount;

    public QueryBuilder() {
      this._tableAliases = new Dictionary<Type, string>();
      this._parameters = new Dictionary<string, object>();
      this._parameterCount = 0;
      this._tableCount = 0;
    }

    public abstract string GetSql();

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
