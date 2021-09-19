using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System;

namespace Zeus {

  public static class TableDefinitionCache {

    private static readonly ConcurrentDictionary<Type, TableDefinition> tableDefinitions;
    private static readonly Dictionary<Type, SqlDbType> mappedDbTypes;

    static TableDefinitionCache() {
      tableDefinitions = new ConcurrentDictionary<Type, TableDefinition>();
      mappedDbTypes = new Dictionary<Type, SqlDbType>() {
        { typeof(long), SqlDbType.BigInt },
        { typeof(bool), SqlDbType.Bit },
        { typeof(byte[]), SqlDbType.Binary },
        { typeof(string), SqlDbType.VarChar },
        { typeof(DateTime), SqlDbType.DateTime2 },
        { typeof(decimal), SqlDbType.Decimal },
        { typeof(double), SqlDbType.Float },
        { typeof(int), SqlDbType.Int },
        { typeof(float), SqlDbType.Real },
        { typeof(short), SqlDbType.SmallInt },
        { typeof(byte), SqlDbType.TinyInt },
        { typeof(Guid), SqlDbType.UniqueIdentifier }
      };
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
              columnDefinitions.Add(
                new ColumnDefinition(
                  columnAttribute?.Name ?? propertyInfo.Name,
                  InterpretDbTypeFromPropertyInfo(propertyInfo),
                  propertyInfo
                )
              );
            }
          }
          TableDefinition tableDefinition = new TableDefinition(type, tableAttribute.Name ?? type.Name, columnDefinitions);
          tableDefinitions.TryAdd(type, tableDefinition);
          return tableDefinition;
        }
      }
    }

    private static SqlDbType InterpretDbTypeFromPropertyInfo(PropertyInfo propertyInfo) {
      Type underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
      if (mappedDbTypes.TryGetValue(underlyingType, out SqlDbType dbType)) {
        return dbType;
      } else {
        throw new InvalidColumnDataTypeException();
      }
    }
  }
}
