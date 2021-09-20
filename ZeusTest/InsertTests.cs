using Xunit;

namespace ZeusTest {

  public class InsertTests {

    [Fact]
    public void TestSingleInsert() {
      TestModel test = new TestModel() {
        Data = "test",
        test_data = "data"
      };
      Assert.True(Repository.Database.Save(test));
    }

    [Fact]
    public void TestInsertWithNullData() {
      TestModel test = new TestModel() {
        Data = null,
        test_data = "test"
      };
      Assert.True(Repository.Database.Save(test));
    }

    [Fact]
    public void TestInsertWithInvalidData() {
      TestModel test = new TestModel() {
        Data = new string('c', 300),
        test_data = null
      };
      try {
        Repository.Database.Save(test);
        Assert.True(false);
      } catch { }
    }
  }
}
