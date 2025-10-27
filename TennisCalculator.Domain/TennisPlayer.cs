namespace TennisCalculator.Domain;

public record TennisPlayer
{
    public required string Name { get; init; }
    
    public virtual bool Equals(TennisPlayer? other) => 
        other is not null && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(TennisPlayer player, string? playerName)
    {
        return player.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase);
    }
    public static bool operator !=(TennisPlayer player, string? playerName)
    {
        return !player.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase);
    }
}