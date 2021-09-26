namespace ZeusMigrations.Models {

  public class ColumnDefinition {

    private SqlDataType _dataType;
    private string _columnName;

    public ColumnDefinition(string columnName, SqlDataType dataType) {
      this._columnName = columnName;
      this._dataType = dataType;
    }

    public override string ToString() {
      return $"{this._columnName} {this._dataType}";
    }
  }
}
