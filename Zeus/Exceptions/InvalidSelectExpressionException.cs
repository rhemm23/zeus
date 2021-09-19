using System.Linq.Expressions;
using System;

namespace Zeus {

  class InvalidSelectExpressionException : Exception {

    public InvalidSelectExpressionException(Expression expression) : base(expression.ToString()) { }
  }
}
