namespace Belmondo.ZeroInsertionForce;

public class Game
{
    public State? CurrentState;
    public World? World;

    public void Reset()
    {
        CurrentState?.Reset();
        World = default;
    }

    public void Update(double deltaSeconds) => CurrentState?.Update(deltaSeconds);
}
