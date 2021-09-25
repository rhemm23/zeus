using System.Data;

namespace ZeusMigrations.Models {

  public class ColumnDefinition {

    public bool IsNotNull { get; set; }

    private SqlDataType _dataType;
    private string _columnName;

    public ColumnDefinition(string columnName, SqlDataType dataType) {
      this._columnName = columnName;
      this._dataType = dataType;
    }
  }
}
