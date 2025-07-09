namespace Belmondo.ZeroInsertionForce;

public sealed class CommandSequence
{
    public Dictionary<double, List<Command>> Commands = [];

    public int Record(double time, in Command command)
    {
        if (!Commands.TryGetValue(time, out List<Command>? commands))
        {
            commands = [];
            Commands.Add(time, commands);
        }

        var index = commands.Count;

        commands.Add(command);

        return index;
    }

    public void TryExecuteAllAt(double time)
    {
        if (Commands.TryGetValue(time, out var commands))
        {
            foreach (var command in commands)
            {
                command.Execute();
            }
        }
    }
}
