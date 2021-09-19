using System.Collections.Generic;
using System.Text;

namespace Zeus.Tokens.Select {

  class SelectQuerySpecification : IWriteSql {

    private List<SelectItem> _selectItems;
    private TableSource _tableSource;

    public SelectQuerySpecification(TableSource tableSource, List<SelectItem> selectItems) {
      this._selectItems = selectItems;
      this._tableSource = tableSource;
    }

    public void WriteSql(StringBuilder sql) {
      sql.Append("SELECT ");
      for (int i = 0; i < this._selectItems.Count; i++) {
        if (i > 0) {
          sql.Append(", ");
        }
        this._selectItems[i].WriteSql(sql);
      }
      sql.Append(" FROM ");
      this._tableSource.WriteSql(sql);
    }
  }
}
