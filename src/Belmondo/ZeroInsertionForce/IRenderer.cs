namespace Belmondo.ZeroInsertionForce;

public interface IRenderer<TGameState>
{
    void Render(in TGameState gameState, double deltaTimeSeconds);
}
