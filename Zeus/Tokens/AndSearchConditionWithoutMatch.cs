using System.Text;

namespace Zeus.Tokens {

  class AndSearchConditionWithoutMatch : SearchConditionWithoutMatch {

    private enum BType { 
      SearchConditionWithoutMatch,
      Predicate
    }

    private SearchConditionWithoutMatch _searchConditionWithoutMatchB;
    private Predicate _predicateB;

    private BType _bType;

    public AndSearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatchA, SearchConditionWithoutMatch searchConditionWithoutMatchB) : base(searchConditionWithoutMatchA) {
      this._searchConditionWithoutMatchB = searchConditionWithoutMatchB;
      this._bType = BType.SearchConditionWithoutMatch;
    }

    public AndSearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatchA, Predicate predicateB) : base(searchConditionWithoutMatchA) {
      this._predicateB = predicateB;
      this._bType = BType.Predicate;
    }

    public AndSearchConditionWithoutMatch(Predicate predicateA, SearchConditionWithoutMatch searchConditionWithoutMatchB) : base(predicateA) {
      this._searchConditionWithoutMatchB = searchConditionWithoutMatchB;
      this._bType = BType.SearchConditionWithoutMatch;
    }

    public AndSearchConditionWithoutMatch(Predicate predicateA, Predicate predicateB) : base(predicateA) {
      this._predicateB = predicateB;
      this._bType = BType.Predicate;
    }

    public override void WriteSql(StringBuilder sql) {
      base.WriteSql(sql);
      sql.Append(" AND ");
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
