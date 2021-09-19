using System.Reflection;

namespace Zeus {

  public class ColumnDefinition {

    public PropertyInfo PropertyInfo { get; }

    public string Name { get; }

    public ColumnDefinition(string name, PropertyInfo propertyInfo) {
      this.PropertyInfo = propertyInfo;
      this.Name = name;
    }
  }
}
