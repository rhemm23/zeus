using System.Collections.Generic;
using ZeusMigrations.Actions;
using System.Data.SqlClient;
using System;

namespace ZeusMigrations {

  public class MigrationManager {

    private IEnumerable<Migration> _migrations;
    private string _databaseUrl;

    public MigrationManager(string databaseUrl, IEnumerable<Migration> migrations) {
      this._databaseUrl = databaseUrl;
      this._migrations = migrations;
    }

    public void ExecutePendingMigrations() {
      using (SqlConnection conn = this.GetNewConnectionAndOpen()) {
        HashSet<Guid> executedMigrationIds = GetExecutedMigrations();
        foreach (Migration migration in this._migrations) {
          if (executedMigrationIds.Contains(migration.Id)) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Already executed migration {migration.Id}");
          } else {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Executing migration {migration.Id}");
            SqlTransaction migrationTransaction = conn.BeginTransaction();
            try {
              foreach (IAction action in migration.Actions()) {
                Console.Write("  ");
                action.PrintUpDescription();
                using (SqlCommand command = new SqlCommand(action.GetUpQuery(), conn)) {
                  command.Transaction = migrationTransaction;
                  command.ExecuteNonQuery();
                }
                InsertIntoMigrationTable(migration.Id);
              }
              Console.WriteLine("\nSuccessfully completed all pending migrations. Database is up to date");
            } catch {
              try {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Migration failed, attempting to rollback");
                migrationTransaction.Rollback();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully rolled back migration");
              } catch {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to rollback transaction after migration failed");
              }
            }
          }
        }
      }
    }

    private bool InsertIntoMigrationTable(Guid migrationId) {
      using (SqlConnection conn = this.GetNewConnectionAndOpen()) {
        string query = "INSERT INTO migrations (migration_id) VALUES (@MigrationId);";
        using (SqlCommand command = new SqlCommand(query, conn)) {
          command.Parameters.AddWithValue("@MigrationId", migrationId.ToString());
          return command.ExecuteNonQuery() == 1;
        }
      }
    }

    private HashSet<Guid> GetExecutedMigrations() {
      CreateMigrationTableIfNotExist();

      HashSet<Guid> migrationIds = new HashSet<Guid>();
      SqlDataReader reader = ExecuteReader("SELECT migration_id FROM migrations;");
      while (reader.Read()) {
        if (Guid.TryParse(reader.GetString(0), out Guid migrationId)) {
          migrationIds.Add(migrationId);
        }
      }
      return migrationIds;
    }

    private void CreateMigrationTableIfNotExist() {
      ExecuteNonQuery("IF OBJECT_ID('migrations') IS NULL CREATE TABLE migrations (id int IDENTITY(1,1) PRIMARY KEY, migration_id varchar(40) NOT NULL);");
    }

    private SqlDataReader ExecuteReader(string query) {
      using (SqlConnection conn = this.GetNewConnectionAndOpen()) {
        using (SqlCommand command = new SqlCommand(query, conn)) {
          return command.ExecuteReader();
        }
      }
    }

    private SqlConnection GetNewConnectionAndOpen() {
      SqlConnection connection = new SqlConnection(this._databaseUrl);
      connection.Open();
      return connection;
    }

    private void ExecuteNonQuery(string query) {
      using (SqlConnection conn = this.GetNewConnectionAndOpen()) {
        using (SqlCommand command = new SqlCommand(query, conn)) {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}
