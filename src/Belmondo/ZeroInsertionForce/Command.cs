namespace Belmondo.ZeroInsertionForce;

public struct Command
{
    public required Action Execute;
    public Action? Undo;
}
