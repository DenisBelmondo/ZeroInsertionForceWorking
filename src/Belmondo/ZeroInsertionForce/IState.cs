namespace Belmondo.ZeroInsertionForce;

public interface IState : IResettable
{
    void Enter();
    void Update(double deltaSeconds);
    void Exit();
}
