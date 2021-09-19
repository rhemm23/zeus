using Zeus.Tokens.Expressions;
using System.Text;

namespace Zeus.Tokens.Predicates {

  class IsNullPredicate : Predicate {

    private Expression _expression;

    public IsNullPredicate(Expression expression) {
      this._expression = expression;
    }

    public override void WriteSql(StringBuilder sql) {
      this._expression.WriteSql(sql);
      sql.Append(" IS NULL");
    }
  }
}
