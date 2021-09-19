using Zeus.Tokens.Expressions;
using System.Text;

namespace Zeus.Tokens.Predicates {

  class ComparisonPredicate : Predicate {

    public enum Operator {
      Equal,
      NotEqual,
      GreaterThan,
      GreaterThanOrEqual,
      NotGreaterThan,
      LessThan,
      LessThanOrEqual,
      NotLessThan
    }

    private Expression _expressionA;
    private Expression _expressionB;

    private Operator _operator;

    public ComparisonPredicate(Operator op, Expression expressionA, Expression expressionB) {
      this._expressionA = expressionA;
      this._expressionB = expressionB;
      this._operator = op;
    }

    public override void WriteSql(StringBuilder sql) {
      this._expressionA.WriteSql(sql);
      switch (this._operator) {
        case Operator.Equal:
          sql.Append(" = ");
          break;

        case Operator.NotEqual:
          sql.Append(" != ");
          break;

        case Operator.GreaterThan:
          sql.Append(" > ");
          break;

        case Operator.GreaterThanOrEqual:
          sql.Append(" >= ");
          break;

        case Operator.NotGreaterThan:
          sql.Append(" !> ");
          break;

        case Operator.LessThan:
          sql.Append(" < ");
          break;

        case Operator.LessThanOrEqual:
          sql.Append(" <= ");
          break;

        case Operator.NotLessThan:
          sql.Append(" !< ");
          break;
      }
      this._expressionB.WriteSql(sql);
    }
  }
}
