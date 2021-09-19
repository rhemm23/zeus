using System.Reflection;
using System.Data;

namespace Zeus {

  public class ColumnDefinition {

    public PropertyInfo PropertyInfo { get; }

    public SqlDbType DbType { get; }

    public string Name { get; }

    public ColumnDefinition(string name, SqlDbType dbType, PropertyInfo propertyInfo) {
      this.PropertyInfo = propertyInfo;
      this.DbType = dbType;
      this.Name = name;
    }
  }
}
