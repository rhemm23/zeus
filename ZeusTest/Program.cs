using System;
using System.Diagnostics;
using Zeus;

namespace ZeusTest {
  class Program {

    private static readonly string DB_CONN_STR = "Data Source=DESKTOP;Initial Catalog=zeustest;User Id=admin;Password=admin;";

    static void Main() {
      Database db = new Database(DB_CONN_STR);

      // Load caches
      foreach (User user in db.Select<User>(user => user.ID, user => user.LastName).All()) {
        Console.WriteLine(user);
      }

      // Now test for speed
      Stopwatch sw = Stopwatch.StartNew();
      var users = db.Select<User>(user => user.ID, user => user.FirstName).All();
      sw.Stop();

      Console.WriteLine(sw.ElapsedMilliseconds);

      Console.ReadKey();
    }
  }

  [Table(Name = "users")]
  class User {

    [Column(Name = "id")]
    public int ID { get; set; }

    [Column(Name = "first_name")]
    public string FirstName { get; set; }

    [Column(Name = "last_name")]
    public string LastName { get; set; }

    [Column(Name = "username")]
    public string Username { get; set; }
  }
}
