namespace Belmondo.ZeroInsertionForce;

public class CommandSequence
{
    public Dictionary<double, List<Command>> Commands = [];

    public void Record(double time, in Command command)
    {
        if (!Commands.TryGetValue(time, out List<Command>? commands))
        {
            commands = [];
            Commands.Add(time, commands);
        }

        commands.Add(command);
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
