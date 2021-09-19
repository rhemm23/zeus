using System.Text;

namespace Zeus.Tokens.Expressions {

  class ParameterExpression : Expression {

    private string _parameterName;

    public ParameterExpression(string parameterName) {
      this._parameterName = parameterName;
    }

    public override void WriteSql(StringBuilder sql) {
      sql.Append($"@{this._parameterName}");
    }
  }
}
