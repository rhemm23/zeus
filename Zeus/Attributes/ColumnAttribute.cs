using System.Data;
using System;

namespace Zeus {

  [AttributeUsage(AttributeTargets.Property)]
  public class ColumnAttribute : Attribute {

    public SqlDbType DataType { get; set; }

    public bool IsPrimaryKey { get; set; }

    public string Name { get; set; }
  }
}
