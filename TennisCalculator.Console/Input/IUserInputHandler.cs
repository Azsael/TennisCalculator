namespace TennisCalculator.Console.Input;

internal interface IUserInputHandler
{
    Task<string?> Handle(string input, CancellationToken cancellationToken = default);
}

