namespace Belmondo.ZeroInsertionForce;

public class State : IState
{
    public event Action? Entered;
    public event Action? Exited;

    public Action? ResetDelegate;
    public Action? EnterDelegate;
    public Func<double, bool>? UpdateDelegate;
    public Action? ExitDelegate;

    public void Reset() => ResetDelegate?.Invoke();

    public void Enter()
    {
        EnterDelegate?.Invoke();
        Entered?.Invoke();
    }

    public void Update(double delta) => UpdateDelegate?.Invoke(delta);

    public void Exit()
    {
        ExitDelegate?.Invoke();
        Exited?.Invoke();
    }
}
