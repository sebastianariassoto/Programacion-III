namespace CampusNet.Models;

/// <summary>Arista dirigida: From sigue a To (From -> To).</summary>
public record Edge(string From, string To)
{
    public override string ToString() => $"{From} -> {To}";
}
