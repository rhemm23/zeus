# zeus

Zeus is a simple Sql ORM for C#

To use, first define a data class

```
[Table(Name = "dogs")]
public class Dog {

  [Column(Name = "id", IsPrimaryKey = true)]
  public int ID { get; set; }
  
  public string Name { get; set; }
  
  public int AgeInDogYears { get; set; }
  
  [Ignore]
  public int AgeInHumanYears => this.AgeInDogYears * 7;
}
```

The first step is adding the table attribute, which is required for all database record classes.

Note the name parameter is optional, and if one isn't supplied the class name is used.

The next step is to add any column or ignore attributes to public properties.

By default, all public properties of the class are treated as database columns.

The column attribute can be used to specify a specific column name for a property.

It can also be used to define if a property is a primary key, and at least one primary key must be defined per class.

The ignore attribute can be used to exclude public properties from being treated as columns.

```
public static void Main() {
  Database db = new Database("...connection string...");
  
  Dog dog = new Dog() { Name = "Fido" };
  
  db.Save(dog);
  
  var myNewDog = db.Select<Dog>(x => x.Name).Where(x => x.Name == "Fido").First();
  
  myNewDog.AgeInDogYears = 3;
  
  db.Save(myNewDog);
  
  db.Delete(myNewDog);
}
```

The next step is to construct an instance of database. This is the primary object to perform database operations.

Inserting, updating, deleting, and querying are all supported. 

The Save method can be used to either insert or update. Which one will be determined by looking at the primary key value.

Select has support for selecting specific columns, or filtering using predicates. You can also order the results by a column using OrderByAsc or OrderByDesc.

Finally delete will delete only the record you pass in.
