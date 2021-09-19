using System.Text;

namespace Zeus.Tokens.Predicates {

  class IsNotNullPredicate : Predicate {

    private Expression _expression;

    public IsNotNullPredicate(Expression expression) {
      this._expression = expression;
    }

    public override void WriteSql(StringBuilder sql) {
      this._expression.WriteSql(sql);
      sql.Append(" IS NOT NULL");
    }
  }
}
