using System.Linq.Expressions;
using Zeus.Tokens;
using System;

namespace Zeus.ExpressionInterpreters {

  class ConditionExpressionInterpreter<T> : ExpressionVisitor {

    private Expression<Predicate<T>> _condition;

    public ConditionExpressionInterpreter(Expression<Predicate<T>> condition) {
      this._condition = condition;
    }

    public SearchCondition GetSearchCondition() {
      this.Visit(this._condition);
      throw new NotImplementedException();
    }
  }
}
