namespace Zeus.ExpressionInterpreters.ExpressionTranslations {

  class ColumnAccessExpressionTranslation : ExpressionTranslation {

    public override ExpressionTranslationType Type => ExpressionTranslationType.ColumnAccess;

    public ColumnDefinition ColumnDefinition { get; }

    public ColumnAccessExpressionTranslation(ColumnDefinition columnDefinition) {
      this.ColumnDefinition = columnDefinition;
    }
  }
}
