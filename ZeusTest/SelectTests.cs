using Xunit;

namespace ZeusTest {

  public class SelectTests {

    [Fact]
    public void TestFirst() {
      string data = "data";
      TestModel test = new TestModel() {
        Data = data,
        test_data = "zeus"
      };

      Assert.True(Repository.Database.Save(test));
      Assert.True(test.ID > 0);

      var model = Repository.Database.Select<TestModel>().Where(x => x.ID == test.ID).First();

      Assert.True(model != null && model.ID == test.ID);
    }
  }
}
