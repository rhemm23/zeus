using Zeus;

namespace ZeusTest {

  public static class Repository {

    public static Database Database;

    static Repository() {
      Database = new Database("Server=DESKTOP;Database=zeustest;User Id=admin;Password=admin;");
    }
  }
}
