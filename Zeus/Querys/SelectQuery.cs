using Zeus.ExpressionInterpreters;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SqlClient;
using Zeus.QueryBuilders;
using Zeus.Tokens.Select;
using Zeus.Tokens;
using System;

namespace Zeus.Querys {

  public class SelectQuery<T> : Query<T> {

    private SelectQueryBuilder _selectQueryBuilder;

    public SelectQuery(SqlConnection connection, IEnumerable<Expression<Func<T, object>>> selectStatements) : base(connection) {
      this._selectQueryBuilder = InitializeSelectQueryBuilder(selectStatements);
    }

    public SelectQuery<T> Where(Expression<Predicate<T>> condition) {
      ConditionExpressionInterpreter<T> conditionExpressionInterpreter = new ConditionExpressionInterpreter<T>(condition);
      this._selectQueryBuilder.Where(conditionExpressionInterpreter.GetSearchCondition());
      return this;
    }

    public override string GetSql() {
      return this._selectQueryBuilder.GetSql();
    }

    private SelectQueryBuilder InitializeSelectQueryBuilder(IEnumerable<Expression<Func<T, object>>> selectStatements) {
      TableDefinition tableDefinition = TableDefinitionCache.GetTableDefinition(typeof(T));
      SelectQueryBuilder selectQueryBuilder = new SelectQueryBuilder();

      string tableAlias = selectQueryBuilder.GetTableAlias(typeof(T));

      selectQueryBuilder.From(
        new TableSource(tableDefinition.Name, tableAlias)
      );
      List<SelectItem> columnSelects = new List<SelectItem>();
      foreach (Expression<Func<T, object>> selectStatement in selectStatements) {
        columnSelects.Add(
          new SelectColumn(tableAlias, this.GetColumnName(selectStatement))
        );
      }
      selectQueryBuilder.Select(columnSelects);
      return selectQueryBuilder;
    }

    private string GetColumnName(Expression<Func<T, object>> selectExpression) {
      SelectExpressionInterpreter<T> expressionInterpreter = new SelectExpressionInterpreter<T>(selectExpression);
      return expressionInterpreter.GetColumnName();
    }
  }
}
