using System.Text;

namespace Zeus.Tokens.Predicates {

  class NotLikePredicate : Predicate {

    private StringExpression _expressionA;
    private StringExpression _expressionB;

    private char? _escapeCharacter;

    public NotLikePredicate(StringExpression expressionA, StringExpression expressionB) {
      this._expressionA = expressionA;
      this._expressionB = expressionB;
    }

    public NotLikePredicate(StringExpression expressionA, StringExpression expressionB, char escapeCharacter) {
      this._escapeCharacter = escapeCharacter;
      this._expressionA = expressionA;
      this._expressionB = expressionB;
    }

    public override void WriteSql(StringBuilder sql) {
      this._expressionA.WriteSql(sql);
      sql.Append(" NOT LIKE ");
      this._expressionB.WriteSql(sql);

      if (this._escapeCharacter != null) {
        sql.Append($" ESCAPE '{this._escapeCharacter.Value}'");
      }
    }
  }
}
