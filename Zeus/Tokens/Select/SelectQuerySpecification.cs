using System.Collections.Generic;
using System.Text;

namespace Zeus.Tokens.Select {

  class SelectQuerySpecification : IWriteSql {

    public List<SelectItem> SelectItems { get; set; }

    public TableSource TableSource { get; set; }

    public SearchCondition Where { get; set; }

    public Expression Top { get; set; }

    public void WriteSql(StringBuilder sql) {
      sql.Append("SELECT ");
      if (this.Top != null) {
        sql.Append("TOP (");
        this.Top.WriteSql(sql);
        sql.Append(") ");
      }
      for (int i = 0; i < this.SelectItems.Count; i++) {
        if (i > 0) {
          sql.Append(", ");
        }
        this.SelectItems[i].WriteSql(sql);
      }
      sql.Append(" FROM ");
      this.TableSource.WriteSql(sql);
      if (this.Where != null) {
        sql.Append(" WHERE ");
        this.Where.WriteSql(sql);
      }
    }
  }
}
