using System.Text;

namespace Zeus.Tokens.Predicates {

  abstract class Predicate : IWriteSql {

    public abstract void WriteSql(StringBuilder sql);
  }
}
