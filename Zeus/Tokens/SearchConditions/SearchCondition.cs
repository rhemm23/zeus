using System.Text;

namespace Zeus.Tokens.SearchConditions {

  class SearchCondition : IWriteSql {

    private enum SearchConditionType {
      GraphSearchCondition,
      SearchConditionWithoutMatch,
      AndSearchCondition
    }

    private SearchConditionWithoutMatch _searchConditionWithoutMatch;
    private GraphSearchPattern _graphSearchPattern;

    private SearchCondition _searchConditionA;
    private SearchCondition _searchConditionB;

    private SearchConditionType _searchConditionType;

    public SearchCondition(GraphSearchPattern graphSearchPattern) {
      this._searchConditionType = SearchConditionType.GraphSearchCondition;
      this._graphSearchPattern = graphSearchPattern;
    }

    public SearchCondition(SearchConditionWithoutMatch searchConditionWithoutMatch) {
      this._searchConditionType = SearchConditionType.SearchConditionWithoutMatch;
      this._searchConditionWithoutMatch = searchConditionWithoutMatch;
    }

    public SearchCondition(SearchCondition searchConditionA, SearchCondition searchConditionB) {
      this._searchConditionType = SearchConditionType.AndSearchCondition;
      this._searchConditionA = searchConditionA;
      this._searchConditionB = searchConditionB;
    }

    public void WriteSql(StringBuilder sb) {
      switch (this._searchConditionType) {
        case SearchConditionType.GraphSearchCondition:
          sb.Append("MATCH(");
          this._graphSearchPattern.WriteSql(sb);
          sb.Append(")");
          break;

        case SearchConditionType.SearchConditionWithoutMatch:
          this._searchConditionWithoutMatch.WriteSql(sb);
          break;

        case SearchConditionType.AndSearchCondition:
          this._searchConditionA.WriteSql(sb);
          sb.Append(" AND ");
          this._searchConditionB.WriteSql(sb);
          break;
      }
    }
  }
}
