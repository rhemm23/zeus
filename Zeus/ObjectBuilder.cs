using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
using System;

namespace Zeus {

  public class ObjectBuilder {

    private Func<object[], object> _objectInitializerFunction;
    private Dictionary<string, int> _columnOrderByName;
    private int _columnCount;

    public ObjectBuilder(Type type) {
      this._columnCount = TableDefinitionCache.GetTableDefinition(type).ColumnDefinitions.Count;
      this._columnOrderByName = new Dictionary<string, int>();

      this._objectInitializerFunction = BuildObjectInitializerFunction(type);
    }

    public object InitializeObjectFromDataRecord(IDataRecord dataRecord) {
      object[] data = new object[this._columnCount];
      for (int i = 0; i < dataRecord.FieldCount; i++) {
        if (this._columnOrderByName.TryGetValue(dataRecord.GetName(i), out int order)) {
          data[order] = dataRecord.GetValue(i);
        }
      }
      return this._objectInitializerFunction(data);
    }

    private Func<object[], object> BuildObjectInitializerFunction(Type type) {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(type);
      ParameterExpression columnDataExpression = Expression.Parameter(typeof(object[]));
      List<MemberBinding> memberBindings = new List<MemberBinding>();
      for (int i = 0; i < tableDefinition.ColumnDefinitions.Count; i++) {
        memberBindings.Add(
          Expression.Bind(
            tableDefinition.ColumnDefinitions[i].PropertyInfo,
            Expression.Convert(
              Expression.ArrayAccess(
                columnDataExpression,
                Expression.Constant(i)
              ),
              tableDefinition.ColumnDefinitions[i].PropertyInfo.PropertyType
            )
          )
        );
        this._columnOrderByName[tableDefinition.ColumnDefinitions[i].Name] = i;
      }
      Expression<Func<object[], object>> objectInitializerFunction = Expression.Lambda<Func<object[], object>>(
        Expression.MemberInit(
          Expression.New(type),
          memberBindings
        ),
        columnDataExpression
      );
      return objectInitializerFunction.Compile();
    }
  }
}
