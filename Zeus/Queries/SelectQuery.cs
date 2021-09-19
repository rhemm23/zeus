using Zeus.ExpressionInterpreters;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SqlClient;
using Zeus.QueryBuilders;
using Zeus.Tokens.Select;
using Zeus.Tokens;
using System;
using System.Linq;

namespace Zeus.Queries {

  public class SelectQuery<T> : Query<T> {

    private SelectQueryBuilder _selectQueryBuilder;

    public SelectQuery(SqlConnection connection, IEnumerable<Expression<Func<T, object>>> selectStatements) : base(connection) {
      this._selectQueryBuilder = InitializeSelectQueryBuilder(selectStatements);
    }

    public SelectQuery<T> Top(int count) {
      Tokens.Expressions.ParameterExpression topParameterExpression = this._selectQueryBuilder.AddParameter(count);
      this._selectQueryBuilder.Top(topParameterExpression);
      return this;
    }

    public override T First() {
      this.Top(1);
      return base.First();
    }

    public SelectQuery<T> Where(Expression<Predicate<T>> condition) {
      ConditionExpressionInterpreter<T> conditionExpressionInterpreter = new ConditionExpressionInterpreter<T>(this._selectQueryBuilder, condition);
      this._selectQueryBuilder.Where(conditionExpressionInterpreter.GetSearchCondition());
      return this;
    }

    public override SqlCommand GetSqlCommand() {
      return this._selectQueryBuilder.GetSqlCommand();
    }

    private SelectQueryBuilder InitializeSelectQueryBuilder(IEnumerable<Expression<Func<T, object>>> selectStatements) {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
      SelectQueryBuilder selectQueryBuilder = new SelectQueryBuilder(typeof(T));

      string tableAlias = selectQueryBuilder.GetTableAlias(typeof(T));
      selectQueryBuilder.From(new TableSource(tableDefinition.Name, tableAlias));

      if (selectStatements.Count() > 0) {
        List<SelectItem> columnSelects = new List<SelectItem>();
        foreach (Expression<Func<T, object>> selectStatement in selectStatements) {
          SelectExpressionInterpreter<T> expressionInterpreter = new SelectExpressionInterpreter<T>(selectQueryBuilder, selectStatement);
          columnSelects.Add(expressionInterpreter.GetSelectItem());
        }
        selectQueryBuilder.Select(columnSelects);
      }
      return selectQueryBuilder;
    }
  }
}
