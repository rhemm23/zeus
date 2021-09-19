using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Zeus {

  public static class TableDefinitionCache {

    private static readonly ConcurrentDictionary<Type, TableDefinition> tableDefinitions;

    static TableDefinitionCache() {
      tableDefinitions = new ConcurrentDictionary<Type, TableDefinition>();
    }

    public static TableDefinition GetTableDefinition(Type type) {
      if (tableDefinitions.TryGetValue(type, out TableDefinition Definition)) {
        return Definition;
      } else {
        TableAttribute tableAttribute = type.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
        if (tableAttribute == null) {
          throw new MissingTableAttributeException();
        } else {
          List<ColumnDefinition> columnDefinitions = new List<ColumnDefinition>();
          foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
            if (propertyInfo.GetCustomAttribute(typeof(IgnoreAttribute)) == null) {
              ColumnAttribute columnAttribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
              columnDefinitions.Add(new ColumnDefinition(columnAttribute?.Name ?? propertyInfo.Name, propertyInfo));
            }
          }
          TableDefinition tableDefinition = new TableDefinition(type, tableAttribute.Name ?? type.Name, columnDefinitions);
          tableDefinitions.TryAdd(type, tableDefinition);
          return tableDefinition;
        }
      }
    }
  }
}
