using System.Text;

namespace Zeus.Tokens.Select {

  class SelectQueryExpression : IWriteSql {

    private enum SelectQueryExpressionType {
      SelectQuerySpecification,
      SelectQueryExpression
    }

    private SelectQuerySpecification _selectQuerySpecification;
    private SelectQueryExpression _selectQueryExpression;

    private SelectQueryExpressionType _selectQueryExpressionType;

    public SelectQueryExpression(SelectQuerySpecification selectQuerySpecification) {
      this._selectQueryExpressionType = SelectQueryExpressionType.SelectQuerySpecification;
      this._selectQuerySpecification = selectQuerySpecification;
    }

    public SelectQueryExpression(SelectQueryExpression selectQueryExpression) {
      this._selectQueryExpressionType = SelectQueryExpressionType.SelectQueryExpression;
      this._selectQueryExpression = selectQueryExpression;
    }

    public void WriteSql(StringBuilder sql) {
      switch (this._selectQueryExpressionType) {
        case SelectQueryExpressionType.SelectQueryExpression:
          sql.Append("(");
          this._selectQueryExpression.WriteSql(sql);
          sql.Append(")");
          break;

        case SelectQueryExpressionType.SelectQuerySpecification:
          this._selectQuerySpecification.WriteSql(sql);
          break;
      }
    }
  }
}
