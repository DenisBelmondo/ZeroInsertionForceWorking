namespace Belmondo;

public class State
{
    public event Action? Started;
    public event Action? Ended;

    public Action? StartDelegate;
    public Func<double, bool>? UpdateDelegate;
    public Action? EndDelegate;

    public void Start()
    {
        StartDelegate?.Invoke();
        Started?.Invoke();
    }

    public void Update(double delta) => UpdateDelegate?.Invoke(delta);

    public void End()
    {
        EndDelegate?.Invoke();
        Ended?.Invoke();
    }
}
