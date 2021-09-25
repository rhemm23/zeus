using System.Data.SqlClient;

namespace ZeusMigrations.Actions {

  public interface IAction {

    void Up(SqlConnection connection);

    void Down(SqlConnection connection);
  }
}
