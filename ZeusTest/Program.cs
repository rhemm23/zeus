using System.Diagnostics;
using System;
using Zeus;

namespace ZeusTest {

  class Program {

    private static readonly string DB_CONN_STR = "Data Source=DESKTOP;Initial Catalog=zeustest;User Id=admin;Password=admin;";

    static void Main() {
      Database db = new Database(DB_CONN_STR);
      var users = db.Select<User>()
        .Where(x => x.FirstName == null && x.ID == 1)
        .OrderByDesc(x => x.ID)
        .OrderByAsc(x => x.FirstName)
        .First();

      Test test = new Test();
      test.Data = new Internal();
      test.Data.Data = "doe";

      Stopwatch sw = Stopwatch.StartNew();
      var users2 = db.Select<User>()
        .Where(x => x.IsVerified && x.ID == 1)
        .OrderByDesc(x => x.ID)
        .OrderByAsc(x => x.FirstName)
        .First();

      sw.Stop();

      Console.WriteLine(db.Delete(users2));

      Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");

      Console.ReadKey();
    }
  }

  class Test {
    
    public Internal Data { get; set; }
  }

  class Internal { 
    
    public string Data { get; set; }
  }

  [Table(Name = "users")]
  class User {

    [Column(Name = "id", IsPrimaryKey = true)]
    public int ID { get; set; }

    [Column(Name = "first_name")]
    public string FirstName { get; set; }

    [Column(Name = "last_name")]
    public string LastName { get; set; }

    [Column(Name = "username")]
    public string Username { get; set; }

    [Column(Name = "is_verified")]
    public bool IsVerified { get; set; }
  }
}
