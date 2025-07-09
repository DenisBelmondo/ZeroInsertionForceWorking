namespace Belmondo.ZeroInsertionForce;

public sealed class Game : IResettable
{
    public State? CurrentState;
    public World? World;

    public void Reset()
    {
        World = default;
    }

    public void Update(double deltaSeconds) => CurrentState?.Update(deltaSeconds);
}
