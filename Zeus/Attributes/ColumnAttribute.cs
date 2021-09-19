using System;

namespace Zeus {

  [AttributeUsage(AttributeTargets.Property)]
  public class ColumnAttribute : Attribute {

    public string Name { get; set; }
  }
}
