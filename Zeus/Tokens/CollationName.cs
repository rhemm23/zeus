using System.Text;
using System;

namespace Zeus.Tokens {

  class CollationName : IWriteSql {

    [Flags]
    public enum CollateFlags : short { 
      None = 0,
      CaseInsensitive = 1
    }

    private CollateFlags _collateFlags;

    public CollationName(CollateFlags collateFlags) {
      this._collateFlags = collateFlags;
    }

    public void WriteSql(StringBuilder sql) {
      if (this._collateFlags.HasFlag(CollateFlags.CaseInsensitive)) {
        sql.Append("_CI");
      }
    }
  }
}
