namespace Belmondo.ZeroInsertionForce;

public sealed class CommandSequence
{
    public Dictionary<int, List<Action>> Commands = [];

    public int Record(int time, in Action command)
    {
        if (!Commands.TryGetValue(time, out List<Action>? commands))
        {
            commands = [];
            Commands.Add(time, commands);
        }

        var index = commands.Count;

        commands.Add(command);

        return index;
    }

    public void TryExecuteAllAt(int time)
    {
        if (Commands.TryGetValue(time, out var commands))
        {
            foreach (var command in commands)
            {
                command();
            }
        }
    }
}
