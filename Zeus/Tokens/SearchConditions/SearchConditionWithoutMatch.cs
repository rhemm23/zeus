using Zeus.Tokens.Predicates;
using System.Text;

namespace Zeus.Tokens.SearchConditions {

  class SearchConditionWithoutMatch : IWriteSql {

    private enum SearchConditionWithoutMatchType {
      SearchConditionWithoutMatch,
      Predicate
    }

    private SearchConditionWithoutMatch _searchConditionWithoutMatch;
    private Predicate _predicate;

    private SearchConditionWithoutMatchType _searchConditionWithoutMatchType;

    public SearchConditionWithoutMatch(Predicate predicate) {
      this._searchConditionWithoutMatchType = SearchConditionWithoutMatchType.Predicate;
      this._predicate = predicate;
    }

    public SearchConditionWithoutMatch(SearchConditionWithoutMatch searchConditionWithoutMatch) {
      this._searchConditionWithoutMatchType = SearchConditionWithoutMatchType.SearchConditionWithoutMatch;
      this._searchConditionWithoutMatch = searchConditionWithoutMatch;
    }

    public virtual void WriteSql(StringBuilder sql) {
      switch (this._searchConditionWithoutMatchType) {
        case SearchConditionWithoutMatchType.SearchConditionWithoutMatch:
          sql.Append("(");
          this._searchConditionWithoutMatch.WriteSql(sql);
          sql.Append(")");
          break;

        case SearchConditionWithoutMatchType.Predicate:
          this._predicate.WriteSql(sql);
          break;
      }
    }
  }
}
