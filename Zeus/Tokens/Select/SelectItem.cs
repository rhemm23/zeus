using System.Text;

namespace Zeus.Tokens.Select {

  abstract class SelectItem : IWriteSql {

    public abstract void WriteSql(StringBuilder sql);
  }
}
