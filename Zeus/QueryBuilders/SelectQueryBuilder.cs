using System.Collections.Generic;
using Zeus.Tokens.Select;
using Zeus.Tokens;
using System.Text;

namespace Zeus.QueryBuilders {

  class SelectQueryBuilder : QueryBuilder {

    private SelectQuerySpecification _selectQuerySpecification;

    public SelectQueryBuilder() {
      this._selectQuerySpecification = new SelectQuerySpecification();
    }

    public SelectQueryBuilder From(TableSource tableSource) {
      this._selectQuerySpecification.TableSource = tableSource;
      return this;
    }

    public SelectQueryBuilder Select(List<SelectItem> selectItems) {
      this._selectQuerySpecification.SelectItems = selectItems;
      return this;
    }

    public SelectQueryBuilder Where(SearchCondition searchCondition) {
      this._selectQuerySpecification.Where = searchCondition;
      return this;
    }

    public override string GetSql() {
      SelectStatement selectStatement = new SelectStatement(
        new SelectQueryExpression(
          this._selectQuerySpecification
        )
      );
      StringBuilder sql = new StringBuilder();
      selectStatement.WriteSql(sql);
      return sql.ToString();
    }
  }
}
