namespace Belmondo.ZeroInsertionForce;

public interface IState
{
    public enum Status
    {
        None,
        Running,
        Succeeded,
        Failed,
    }

    void Enter();
    void Update(double deltaSeconds);
    void Exit();
}
