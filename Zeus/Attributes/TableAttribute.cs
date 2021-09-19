using System;

namespace Zeus {

  [AttributeUsage(AttributeTargets.Class)]
  public class TableAttribute : Attribute {

    public string Name { get; set; }
  }
}
