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

    public static SqlDataType VarBinary() {
      return new SqlDataType("varbinary");
    }

    public static SqlDataType VarBinary(int max) {
      return new SqlDataType($"varbinary({max})");
    }

    public static SqlDataType Image() {
      return new SqlDataType("image");
    }

    public static SqlDataType Bit() {
      return new SqlDataType("bit");
    }

    public static SqlDataType TinyInt() {
      return new SqlDataType("tinyint");
    }

    public static SqlDataType SmallInt() {
      return new SqlDataType("smallint");
    }

    public static SqlDataType Int() {
      return new SqlDataType("int");
    }

    public static SqlDataType BigInt() {
      return new SqlDataType("bigint");
    }

    public static SqlDataType Decimal(byte p = 18, byte s = 0) {
      return new SqlDataType($"decimal({p},{s})");
    }

    public static SqlDataType Numeric(byte p = 18, byte s = 0) {
      return new SqlDataType($"numeric({p},{s})");
    }

    public static SqlDataType SmallMoney() {
      return new SqlDataType("smallmoney");
    }

    public static SqlDataType Money() {
      return new SqlDataType("money");
    }

    public static SqlDataType Float(int n = 53) {
      return new SqlDataType($"float({n})");
    }

    public static SqlDataType Real() {
      return new SqlDataType("real");
    }

    public static SqlDataType DateTime() {
      return new SqlDataType("datetime");
    }

    public static SqlDataType DateTime2() {
      return new SqlDataType("datetime2");
    }

    public static SqlDataType SmallDateTime() {
      return new SqlDataType("smalldatetime");
    }

    public static SqlDataType Date() {
      return new SqlDataType("date");
    }

    public static SqlDataType Time() {
      return new SqlDataType("time");
    }

    public static SqlDataType DateTimeOffset() {
      return new SqlDataType("datetimeoffset");
    }

    public static SqlDataType TimeStamp() {
      return new SqlDataType("timestamp");
    }

    public static SqlDataType SqlVariant() {
      return new SqlDataType("sql_variant");
    }

    public static SqlDataType UniqueIdentifier() {
      return new SqlDataType("uniqueidentifier");
    }

    public static SqlDataType Xml() {
      return new SqlDataType("xml");
    }

    public static SqlDataType Cursor() {
      return new SqlDataType("cursor");
    }

    public static SqlDataType Table() {
      return new SqlDataType("table");
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
