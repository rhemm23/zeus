using Zeus;

namespace ZeusTest {

  [Table(Name = "test_models")]
  class TestModel {

    [Column(Name = "id", IsPrimaryKey = true)]
    public int ID { get; set; }

    [Column(Name = "data")]
    public string Data { get; set; }

    public string test_data { get; set; }
  }
}
