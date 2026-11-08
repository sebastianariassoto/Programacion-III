using System.Text;
using CampusNet.Models;

namespace CampusNet.Views;

/// <summary>
/// Vista de consola: solo imprime. No toma decisiones ni modifica el grafo.
/// </summary>
public class ConsoleView
{
    private const int LineWidth = 78;

    public ConsoleView()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    // ------------------------------------------------------------------
    // Mensajes generales
    // ------------------------------------------------------------------

    public void ShowTitle(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', LineWidth));
        Console.WriteLine($"  {text}");
        Console.WriteLine(new string('=', LineWidth));
    }

    public void ShowSection(string text)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {text} ---");
    }

    public void ShowMessage(string text) => Console.WriteLine(text);

    public void ShowSuccess(string text) => Console.WriteLine($"  [OK]    {text}");

    public void ShowError(string text) => Console.WriteLine($"  [ERROR] {text}");

    public void ShowChecklist(IEnumerable<(string Description, bool Passed)> items)
    {
        foreach (var (description, passed) in items)
            Console.WriteLine($"  [{(passed ? "X" : " ")}] {description}");
    }

    // ------------------------------------------------------------------
    // Estructura del grafo
    // ------------------------------------------------------------------

    public void ShowAdjacencyList(Graph graph)
    {
        Console.WriteLine($"  {"Id",-5}{"Nombre",-20}{"Rol",-12}{"Sale",-6}{"Entra",-7}Sigue a");
        Console.WriteLine($"  {new string('-', LineWidth - 4)}");

        foreach (var id in graph.VertexIds)
            ShowVertexRow(graph, id);

        Console.WriteLine();
        Console.WriteLine($"  Total: {graph.VertexCount} vértices | {graph.EdgeCount} aristas");
    }

    public void ShowVertexRow(Graph graph, string id)
    {
        var vertex = graph.GetVertex(id);
        if (vertex == null) return;

        var neighbors = graph.GetNeighbors(vertex.Id);
        string follows = neighbors.Count == 0 ? "(ninguno)" : string.Join(", ", neighbors);

        Console.WriteLine(
            $"  {vertex.Id,-5}{vertex.Name,-20}{vertex.Role,-12}" +
            $"{graph.GetOutDegree(vertex.Id),-6}{graph.GetInDegree(vertex.Id),-7}-> {follows}");
    }

    // ------------------------------------------------------------------
    // Recorridos
    // ------------------------------------------------------------------

    public void ShowBfs(Graph graph, string startId, IReadOnlyList<string> order)
    {
        Console.WriteLine();
        Console.WriteLine($"  BFS desde {Label(graph, startId)}");
        Console.WriteLine($"    Orden de visita : {string.Join(" -> ", order)}");
        Console.WriteLine($"    Vértices alcanzados (incluye el origen): {order.Count} de {graph.VertexCount}");
    }

    public void ShowDfs(DfsResult result)
    {
        Console.WriteLine($"  Orden de descubrimiento: {string.Join(" -> ", result.DiscoveryOrder)}");
        Console.WriteLine($"  Orden de finalización  : {string.Join(" -> ", result.FinishOrder)}");

        if (!result.HasCycle)
        {
            Console.WriteLine("  Ciclos dirigidos: no se detectaron.");
            return;
        }

        Console.WriteLine($"  Ciclos dirigidos detectados: {result.Cycles.Count} (uno por arista de retroceso)");
        int number = 1;
        foreach (var cycle in result.Cycles)
            Console.WriteLine($"    Ciclo {number++}: {string.Join(" -> ", cycle)}");
    }

    // ------------------------------------------------------------------
    // Consultas sociales
    // ------------------------------------------------------------------

    public void ShowUsers(Graph graph, IReadOnlyList<string> ids)
    {
        if (ids.Count == 0)
        {
            Console.WriteLine("    (ninguno)");
            return;
        }

        foreach (var id in ids)
        {
            var vertex = graph.GetVertex(id);
            if (vertex == null) continue;

            Console.WriteLine(
                $"    - {vertex.Id} {vertex.Name} ({vertex.Role}) | " +
                $"seguidores: {graph.GetInDegree(id)} | sigue a: {graph.GetOutDegree(id)}");
        }
    }

    public void ShowReachability(Graph graph, string fromId, string toId, IReadOnlyList<string>? path)
    {
        if (path == null)
        {
            Console.WriteLine($"  {Label(graph, fromId)} NO puede llegar a {Label(graph, toId)}.");
            return;
        }

        Console.WriteLine(
            $"  {Label(graph, fromId)} SÍ puede llegar a {Label(graph, toId)} " +
            $"en {path.Count - 1} salto(s): {string.Join(" -> ", path)}");
    }

    // ------------------------------------------------------------------

    private static string Label(Graph graph, string id)
    {
        var vertex = graph.GetVertex(id);
        return vertex == null ? id : $"{vertex.Id} ({vertex.Name})";
    }
}
