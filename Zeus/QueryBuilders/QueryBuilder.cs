using System.Collections.Generic;
using System;

namespace Zeus.QueryBuilders {

  abstract class QueryBuilder {

    private Dictionary<Type, string> _tableAliases;
    private int _tableCount;

    public QueryBuilder() {
      this._tableAliases = new Dictionary<Type, string>();
      this._tableCount = 0;
    }

    public abstract string GetSql();

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
