using System;
using System.Collections.Generic;
using System.Text;

namespace ZeusMigrations.Models {

  public class SqlDataType {

    public static SqlDataType Char(ushort max) {
      return new SqlDataType($"char({max})");
    }

    public static SqlDataType VarChar(ushort max) {
      return new SqlDataType($"varchar({max})");
    }

    public static SqlDataType VarCharMax() {
      return new SqlDataType("varchar(max)");
    }

    public static SqlDataType Text() {
      return new SqlDataType("text");
    }

    public static SqlDataType NChar(ushort max) {
      return new SqlDataType($"nchar({max})");
    }

    public static SqlDataType NVarChar(ushort max) {
      return new SqlDataType($"nvarchar({max})");
    }

    public static SqlDataType NVarCharMax() {
      return new SqlDataType("nvarchar(max)");
    }

    public static SqlDataType NText() {
      return new SqlDataType("ntext");
    }

    public static SqlDataType Binary(ushort max) {
      return new SqlDataType($"binary({max})");
    }

    private string _type;

    private SqlDataType(string type) {
      this._type = type;
    }

    public override string ToString() {
      return this._type;
    }
  }
}
