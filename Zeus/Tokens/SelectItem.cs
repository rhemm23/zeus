using System.Text;

namespace Zeus.Tokens {

  abstract class SelectItem : IWriteSql {

    public abstract void WriteSql(StringBuilder sql);
  }
}
