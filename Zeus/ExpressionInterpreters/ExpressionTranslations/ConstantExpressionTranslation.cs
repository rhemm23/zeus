namespace Zeus.ExpressionInterpreters.ExpressionTranslations {

  class ConstantExpressionTranslation : ExpressionTranslation {

    public override ExpressionTranslationType Type => ExpressionTranslationType.Constant;

    public object Value { get; }

    public ConstantExpressionTranslation(object value) {
      this.Value = value;
    }
  }
}
