using Zeus.Tokens.SearchConditions;

namespace Zeus.ExpressionInterpreters.ExpressionTranslations {

  class ConditionExpressionTranslation : ExpressionTranslation {

    public override ExpressionTranslationType Type => ExpressionTranslationType.Condition;

    public SearchConditionWithoutMatch Condition { get; }

    public ConditionExpressionTranslation(SearchConditionWithoutMatch condition) {
      this.Condition = condition;
    }
  }
}
