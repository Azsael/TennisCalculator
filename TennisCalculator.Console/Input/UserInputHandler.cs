using Microsoft.Extensions.Logging;
using TennisCalculator.Console.Commands;

namespace TennisCalculator.Console.Input;

internal class UserInputHandler(IEnumerable<IUserCommand> commands, ILogger<UserInputHandler> logger) : IUserInputHandler
{
    /// <summary>
    /// Handles the user input and processes it
    /// </summary>
    /// <param name="input">The user input string</param>
    /// <returns>Any messages for the user</returns>
    public async Task<string?> Handle(string input, CancellationToken cancellationToken = default)
    {
        var splitInput = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var command = commands.FirstOrDefault(c => c.CanHandle(splitInput));

        if (command is null)
        {
            logger.LogWarning("Unknown command: {Input}", input);
            return null;
        }

        return await command.Handle(splitInput, cancellationToken);
    }
}

