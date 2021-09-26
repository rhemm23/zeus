namespace ZeusMigrations.Models {

  public class Identity {

    public static Identity Default;

    static Identity() {
      Default = new Identity(1, 1);
    }

    private int _increment;
    private int _seed;

    public Identity(int seed, int increment) {
      this._increment = increment;
      this._seed = seed;
    }

    public override string ToString() {
      return $"IDENTITY({this._seed},{this._increment})";
    }
  }
}
