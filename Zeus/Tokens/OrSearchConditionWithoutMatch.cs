using Zeus.Tokens.Predicates;
using System.Text;

namespace Zeus.Tokens {

  class OrSearchConditionWithoutMatch : SearchConditionWithoutMatch {

    private enum BType {
      SearchConditionWithoutMatch,
      Predicate
    }

    private SearchConditionWithoutMatch _searchConditionWithoutMatchB;
    private Predicate _predicateB;

    private BType _bType;

    public OrSearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatchA, SearchConditionWithoutMatch searchConditionWithoutMatchB) : base(searchConditionWithoutMatchA) {
      this._searchConditionWithoutMatchB = searchConditionWithoutMatchB;
      this._bType = BType.SearchConditionWithoutMatch;
    }

    public OrSearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatchA, Predicate predicateB) : base(searchConditionWithoutMatchA) {
      this._predicateB = predicateB;
      this._bType = BType.Predicate;
    }

    public OrSearchConditionWithoutMatch(Predicate predicateA, SearchConditionWithoutMatch searchConditionWithoutMatchB) : base(predicateA) {
      this._searchConditionWithoutMatchB = searchConditionWithoutMatchB;
      this._bType = BType.SearchConditionWithoutMatch;
    }

    public OrSearchConditionWithoutMatch(Predicate predicateA, Predicate predicateB) : base(predicateA) {
      this._predicateB = predicateB;
      this._bType = BType.Predicate;
    }

    public override void WriteSql(StringBuilder sql) {
      base.WriteSql(sql);
      sql.Append(" OR ");
      switch (this._bType) {
        case BType.SearchConditionWithoutMatch:
          this._searchConditionWithoutMatchB.WriteSql(sql);
          break;

        case BType.Predicate:
          this._predicateB.WriteSql(sql);
          break;
      }
    }
  }
}
