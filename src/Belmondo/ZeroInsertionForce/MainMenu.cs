namespace Belmondo.ZeroInsertionForce;

public class MainMenu
{
    public enum Option
    {
        None,
        Start,
        Quit,
    }

    public Option CurrentOption;

    public void Reset()
    {
        CurrentOption = Option.Start;
    }

    public MainMenu() => Reset();
    public void SetOption(int option) => CurrentOption = (Option)(option % Enum.GetValues<Option>().Length);
    public void GoToNextOption() => SetOption(((int)CurrentOption) + 1);
    public void GoToPreviousOption() => SetOption(((int)CurrentOption) - 1);
}
