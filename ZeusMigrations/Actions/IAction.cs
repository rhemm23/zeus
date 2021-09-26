namespace ZeusMigrations.Actions {

  public interface IAction {

    string GetUpQuery();

    string GetDownQuery();

    void PrintUpDescription();

    void PrintDownDescription();
  }
}
