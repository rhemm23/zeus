using System.Collections.Generic;
using System.Reflection;
using Zeus.Exceptions;
using System.Linq;
using System;

namespace Zeus {

  public class TableDefinition {

    public Dictionary<PropertyInfo, ColumnDefinition> ColumnDefinitionsByPropertyInfo { get; }

    public List<ColumnDefinition> ColumnDefinitions { get; }

    public ColumnDefinition PrimaryKey { get; }

    public string Name { get; }

    public Type Type { get; }

    public TableDefinition(Type type, string name, List<ColumnDefinition> columnDefinitions) {
      this.ColumnDefinitionsByPropertyInfo = columnDefinitions.ToDictionary(k => k.PropertyInfo, v => v);
      this.PrimaryKey = columnDefinitions.FirstOrDefault(c => c.IsPrimaryKey);
      this.ColumnDefinitions = columnDefinitions;
      this.Name = name;
      this.Type = type;

      if (this.PrimaryKey == null) {
        throw new MissingPrimaryKeyException();
      }
    }
  }
}
