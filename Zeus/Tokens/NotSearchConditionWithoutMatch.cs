using Zeus.Tokens.Predicates;
using System.Text;

namespace Zeus.Tokens {

  class NotSearchConditionWithoutMatch : SearchConditionWithoutMatch {

    public NotSearchConditionWithoutMatch(Predicate predicate) : base(predicate) { }

    public NotSearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatch) : base(searchConditionWithoutMatch) { }

    public override void WriteSql(StringBuilder sql) {
      sql.Append("NOT ");
      base.WriteSql(sql);
    }
  }
}
