using Zeus.Tokens.Expressions;
using System.Text;

namespace Zeus.Tokens.Predicates {

  class BetweenPredicate : Predicate {

    private Expression _lowerBoundExpression;
    private Expression _upperBoundExpression;
    private Expression _expression;

    public BetweenPredicate(Expression expression, Expression lowerBoundExpression, Expression upperBoundExpression) {
      this._lowerBoundExpression = lowerBoundExpression;
      this._upperBoundExpression = upperBoundExpression;
      this._expression = expression;
    }

    public override void WriteSql(StringBuilder sql) {
      this._expression.WriteSql(sql);
      sql.Append(" BETWEEN ");
      this._lowerBoundExpression.WriteSql(sql);
      sql.Append(" AND ");
      this._upperBoundExpression.WriteSql(sql);
    }
  }
}
