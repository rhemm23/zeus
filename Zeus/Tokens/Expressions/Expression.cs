using System.Text;

namespace Zeus.Tokens.Expressions {

  abstract class Expression : IWriteSql {

    public abstract void WriteSql(StringBuilder sql);
  }
}
