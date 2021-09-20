using Zeus.Tokens.SearchConditions;
using System.Collections.Generic;
using Zeus.Tokens.Expressions;
using Zeus.Tokens.Select;
using Zeus.Tokens;
using System.Text;
using System;

namespace Zeus.QueryBuilders {

  class SelectQueryBuilder : QueryBuilder {

    private SelectQuerySpecification _selectQuerySpecification;
    private List<OrderByClause> _orderClauses;

    public SelectQueryBuilder(Type primaryTableType) : base(primaryTableType) {
      this._selectQuerySpecification = new SelectQuerySpecification();
      this._orderClauses = new List<OrderByClause>();
    }

    public SelectQueryBuilder OrderByDesc(Expression expression) {
      this._orderClauses.Add(new OrderByClause(expression, false));
      return this;
    }

    public SelectQueryBuilder OrderByAsc(Expression expression) {
      this._orderClauses.Add(new OrderByClause(expression, true));
      return this;
    }

    public SelectQueryBuilder Top(Expression topExpression) {
      this._selectQuerySpecification.Top = topExpression;
      return this;
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
      if (this._selectQuerySpecification.SelectItems == null) {
        this._selectQuerySpecification.SelectItems = new List<SelectItem>() {
          new SelectAll(this.GetTableAlias(this.PrimaryTableType))
        };
      }
      SelectStatement selectStatement = new SelectStatement(
        new SelectQueryExpression(
          this._selectQuerySpecification
        ),
        this._orderClauses
      );
      StringBuilder sql = new StringBuilder();
      selectStatement.WriteSql(sql);
      return sql.ToString();
    }
  }
}
