namespace CampusNet.Models;

/// <summary>Resultado de un DFS completo sobre el grafo.</summary>
public class DfsResult
{
    /// <summary>Vértices en el orden en que fueron descubiertos.</summary>
    public List<string> DiscoveryOrder { get; } = new();

    /// <summary>Vértices en el orden en que terminó su exploración.</summary>
    public List<string> FinishOrder { get; } = new();

    /// <summary>Ciclos dirigidos detectados (uno por cada arista de retroceso).</summary>
    public List<List<string>> Cycles { get; } = new();

    public bool HasCycle => Cycles.Count > 0;
}
