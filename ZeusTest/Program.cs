﻿using System;
using System.Diagnostics;
using Zeus;

namespace ZeusTest {

  class Program {

    private static readonly string DB_CONN_STR = "Data Source=DESKTOP;Initial Catalog=zeustest;User Id=admin;Password=admin;";

    static void Main() {
      Database db = new Database(DB_CONN_STR);
      var users = db.Select<User>().Where(x => x.FirstName == null && x.ID == 1).First();

      Stopwatch sw = Stopwatch.StartNew();
      var users2 = db.Select<User>().Where(x => x.LastName == "test" && x.ID == 4).First();
      sw.Stop();

      Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms");

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
