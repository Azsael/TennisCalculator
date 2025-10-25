namespace TennisCalculator.Domain;

public record TennisPlayer
{
    public required string Name { get; init; }
    
    public virtual bool Equals(TennisPlayer? other) => 
        other is not null && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        
    public override int GetHashCode() => Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
}