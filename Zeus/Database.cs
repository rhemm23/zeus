using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System;

namespace Zeus {

  public sealed class Database {

    private string _connectionString;

    public Database(string connectionString) {
      this._connectionString = connectionString;
    }

    public SelectQuery<T> Select<T>(params Expression<Func<T, object>>[] selectExpressions) {
      return this.Select<T>(this.GetColumnNames(selectExpressions));
    }

    private SelectQuery<T> Select<T>(IEnumerable<string> columns) {
      return new SelectQuery<T>(this.GetNewConnection(), columns);
    }

    private IEnumerable<string> GetColumnNames<T>(IEnumerable<Expression<Func<T, object>>> selectExpressions) {
      foreach (Expression<Func<T, object>> selectExpression in selectExpressions) {
        yield return GetColumnName(selectExpression);
      }
    }

    private string GetColumnName<T>(Expression<Func<T, object>> selectExpression) {
      SelectExpressionInterpreter<T> expressionInterpreter = new SelectExpressionInterpreter<T>(selectExpression);
      return expressionInterpreter.GetColumnName();
    }

    private SqlConnection GetNewConnection() {
      SqlConnection connection = new SqlConnection(this._connectionString);
      connection.Open();

      return connection;
    }
  }
}
