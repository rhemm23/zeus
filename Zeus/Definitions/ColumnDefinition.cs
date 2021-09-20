using System.Reflection;
using System.Data;

namespace Zeus {

  public class ColumnDefinition {

    public PropertyInfo PropertyInfo { get; }

    public bool IsPrimaryKey { get; }

    public SqlDbType DbType { get; }

    public string Name { get; }

    public ColumnDefinition(string name, SqlDbType dbType, bool isPrimaryKey, PropertyInfo propertyInfo) {
      this.PropertyInfo = propertyInfo;
      this.IsPrimaryKey = isPrimaryKey;
      this.DbType = dbType;
      this.Name = name;
    }
  }
}
