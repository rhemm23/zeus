using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace Zeus {

  public class TableDefinition {

    public Dictionary<PropertyInfo, ColumnDefinition> ColumnDefinitionsByPropertyInfo { get; }

    public List<ColumnDefinition> ColumnDefinitions { get; }

    public string Name { get; }

    public Type Type { get; }

    public TableDefinition(Type type, string name, List<ColumnDefinition> columnDefinitions) {
      this.ColumnDefinitionsByPropertyInfo = columnDefinitions.ToDictionary(k => k.PropertyInfo, v => v);
      this.ColumnDefinitions = columnDefinitions;
      this.Name = name;
      this.Type = type;
    }
  }
}
