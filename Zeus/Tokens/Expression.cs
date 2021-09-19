using System.Text;

namespace Zeus.Tokens {

  abstract class Expression : IWriteSql {

    public abstract void WriteSql(StringBuilder sql);
  }
}
