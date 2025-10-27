namespace TennisCalculator.Console.Commands;

/// <summary>
/// Generic interface for handling queries
/// </summary>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TResult">The result type</typeparam>
public interface IQueryHandler<in TQuery, out TResult>
{
    /// <summary>
    /// Handles the specified query and returns the result
    /// </summary>
    /// <param name="query">The query to handle</param>
    /// <returns>The query result</returns>
    TResult Handle(TQuery query);
}