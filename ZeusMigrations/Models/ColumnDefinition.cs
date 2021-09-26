using System.Text;

namespace ZeusMigrations.Models {

  public class ColumnDefinition {

    public Identity Identity { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsNotNull { get; set; }

    private SqlDataType _dataType;
    private string _columnName;

    public ColumnDefinition(string columnName, SqlDataType dataType) {
      this._columnName = columnName;
      this._dataType = dataType;
    }

    public override string ToString() {
      StringBuilder sql = new StringBuilder($"{this._columnName} {this._dataType}");
      if (this.Identity != null) {
        sql.Append($" {this.Identity}");
      } else if (this.IsNotNull) {
        sql.Append(" NOT NULL");
      }
      if (this.IsPrimaryKey) {
        sql.Append(" PRIMARY KEY");
      }
      return sql.ToString();
    }
  }
}
