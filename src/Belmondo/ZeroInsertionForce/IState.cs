namespace Belmondo.ZeroInsertionForce;

public interface IState
{
    void Enter();
    void Update(double deltaSeconds);
    void Exit();
}
