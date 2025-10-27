using TennisCalculator.Domain;

namespace TennisCalculator.Processing.RawData;

internal interface IMatchDataProcessor
{
    /// <summary>
    /// Creates a TennisMatch entity from raw match data by processing point sequences
    /// </summary>
    /// <param name="rawData">Raw match data containing players and point sequence</param>
    /// <returns>Completed tennis match with all scoring calculated</returns>
    TennisMatch CreateMatchFromRawData(RawMatchData rawData);
}
