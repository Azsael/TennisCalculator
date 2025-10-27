namespace TennisCalculator.Console.Commands;

internal interface IUserCommand
{
    public bool CanHandle(IList<string> command);

    public Task<string?> Handle(IList<string> command, CancellationToken cancellationToken = default);
}

