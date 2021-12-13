using Mechs.Utility.Commands;

#nullable disable

if (args.Length == 0)
{
    Console.WriteLine("Please specify a command.");
    return;
}

var commandName = args[0];

var commandType = typeof(IUtilityCommand);
var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(assembly => assembly.GetTypes())
    .Where(matchingType => commandType.IsAssignableFrom(matchingType));
IUtilityCommand matchingCommand = null;
foreach (var type in commandTypes)
{
    var command = (IUtilityCommand)Activator.CreateInstance(type);
    if (command.CommandName == commandName)
    {
        matchingCommand = command;
        break;
    }
}

if (matchingCommand == null)
{
    Console.WriteLine($"Couldn't find command named {commandName}.");
    return;
}

try
{
    matchingCommand.Execute(args.Skip(1).ToArray());
}
catch (Exception ex)
{
    Console.WriteLine($"Error executing command: {ex.Message}");
}

