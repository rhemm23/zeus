namespace Zeus.ExpressionInterpreters.ExpressionTranslations {

  abstract class ExpressionTranslation {

    public enum ExpressionTranslationType {
      ColumnAccess,
      Condition,
      Constant,
      Null
    }

    public abstract ExpressionTranslationType Type { get; }
  }
}
