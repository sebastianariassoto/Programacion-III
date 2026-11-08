namespace CampusNet.Models;

/// <summary>
/// Grafo dirigido con lista de adyacencia.
/// _adjacency[A] contiene los ids de los usuarios a los que A sigue (A -> B).
/// Esta clase no usa Console: toda la salida es responsabilidad de la View.
/// </summary>
public class Graph
{
    private readonly Dictionary<string, Vertex> _vertices = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<string>> _adjacency = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _order = new(); // orden de inserción de los vértices

    // ------------------------------------------------------------------
    // Consultas básicas
    // ------------------------------------------------------------------

    public int VertexCount => _vertices.Count;

    public int EdgeCount => _adjacency.Values.Sum(list => list.Count);

    /// <summary>Ids de los vértices en orden de inserción.</summary>
    public IReadOnlyList<string> VertexIds => _order.ToList();

    public IReadOnlyList<Vertex> GetVertices() => _order.Select(id => _vertices[id]).ToList();

    public Vertex? GetVertex(string id) => Find(id);

    public bool ContainsVertex(string id) => Find(id) != null;

    public bool ContainsEdge(string fromId, string toId)
    {
        var source = Find(fromId);
        var target = Find(toId);
        return source != null && target != null && _adjacency[source.Id].Contains(target.Id);
    }

    /// <summary>Usuarios a los que sigue el usuario indicado (aristas de salida).</summary>
    public IReadOnlyList<string> GetNeighbors(string id)
    {
        var vertex = Find(id);
        return vertex == null ? new List<string>() : _adjacency[vertex.Id].ToList();
    }

    /// <summary>Usuarios que siguen al usuario indicado (aristas de entrada).</summary>
    public IReadOnlyList<string> GetFollowers(string id)
    {
        var vertex = Find(id);
        if (vertex == null) return new List<string>();
        return _order.Where(other => _adjacency[other].Contains(vertex.Id)).ToList();
    }

    public int GetOutDegree(string id)
    {
        var vertex = Find(id);
        return vertex == null ? 0 : _adjacency[vertex.Id].Count;
    }

    public int GetInDegree(string id)
    {
        var vertex = Find(id);
        return vertex == null ? 0 : _adjacency.Values.Count(list => list.Contains(vertex.Id));
    }

    public List<Edge> GetEdges()
    {
        var edges = new List<Edge>();
        foreach (var id in _order)
            foreach (var target in _adjacency[id])
                edges.Add(new Edge(id, target));
        return edges;
    }

    /// <summary>Todas las aristas que entran o salen del vértice indicado.</summary>
    public List<Edge> GetIncidentEdges(string id)
    {
        var vertex = Find(id);
        if (vertex == null) return new List<Edge>();
        return GetEdges().Where(e => e.From == vertex.Id || e.To == vertex.Id).ToList();
    }

    // ------------------------------------------------------------------
    // CRUD de vértices
    // ------------------------------------------------------------------

    public GraphResult AddVertex(Vertex? vertex)
    {
        if (vertex == null || string.IsNullOrWhiteSpace(vertex.Id) || string.IsNullOrWhiteSpace(vertex.Name))
            return GraphResult.InvalidData;

        if (_vertices.ContainsKey(vertex.Id))
            return GraphResult.VertexExists;

        _vertices[vertex.Id] = vertex;
        _adjacency[vertex.Id] = new List<string>();
        _order.Add(vertex.Id);
        return GraphResult.Ok;
    }

    /// <summary>Elimina el vértice y todas las aristas que entran o salen de él.</summary>
    public GraphResult RemoveVertex(string id)
    {
        var vertex = Find(id);
        if (vertex == null) return GraphResult.VertexNotFound;

        _vertices.Remove(vertex.Id);
        _adjacency.Remove(vertex.Id);
        _order.Remove(vertex.Id);

        foreach (var list in _adjacency.Values)
            list.Remove(vertex.Id);

        return GraphResult.Ok;
    }

    public GraphResult UpdateName(string id, string newName)
    {
        var vertex = Find(id);
        if (vertex == null) return GraphResult.VertexNotFound;
        if (string.IsNullOrWhiteSpace(newName)) return GraphResult.InvalidData;

        vertex.Name = newName.Trim();
        return GraphResult.Ok;
    }

    public GraphResult UpdateRole(string id, Role newRole)
    {
        var vertex = Find(id);
        if (vertex == null) return GraphResult.VertexNotFound;

        vertex.Role = newRole;
        return GraphResult.Ok;
    }

    // ------------------------------------------------------------------
    // CRUD de aristas
    // ------------------------------------------------------------------

    /// <summary>Agrega la arista dirigida fromId -> toId (fromId sigue a toId).</summary>
    public GraphResult AddEdge(string fromId, string toId)
    {
        var source = Find(fromId);
        var target = Find(toId);
        if (source == null || target == null) return GraphResult.VertexNotFound;
        if (source.Id == target.Id) return GraphResult.SelfLoop;

        var neighbors = _adjacency[source.Id];
        if (neighbors.Contains(target.Id)) return GraphResult.EdgeExists;

        neighbors.Add(target.Id);
        return GraphResult.Ok;
    }

    public GraphResult RemoveEdge(string fromId, string toId)
    {
        var source = Find(fromId);
        var target = Find(toId);
        if (source == null || target == null) return GraphResult.VertexNotFound;

        return _adjacency[source.Id].Remove(target.Id) ? GraphResult.Ok : GraphResult.EdgeNotFound;
    }

    // ------------------------------------------------------------------
    // Recorridos
    // ------------------------------------------------------------------

    /// <summary>BFS desde startId. Devuelve los vértices alcanzados en orden de visita.</summary>
    public List<string> Bfs(string startId)
    {
        var order = new List<string>();
        var start = Find(startId);
        if (start == null) return order;

        var visited = new HashSet<string> { start.Id };
        var queue = new Queue<string>();
        queue.Enqueue(start.Id);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var next in _adjacency[current])
            {
                if (visited.Add(next))
                    queue.Enqueue(next);
            }
        }

        return order;
    }

    /// <summary>
    /// DFS completo: se inicia desde cada vértice aún no visitado.
    /// Detecta ciclos dirigidos mediante aristas de retroceso (hacia un vértice
    /// que sigue en el camino actual).
    /// </summary>
    public DfsResult Dfs()
    {
        var result = new DfsResult();
        var state = new Dictionary<string, int>(); // 1 = en el camino actual, 2 = terminado
        var path = new List<string>();

        foreach (var id in _order)
        {
            if (!state.ContainsKey(id))
                DfsVisit(id, state, path, result);
        }

        return result;
    }

    private void DfsVisit(string current, Dictionary<string, int> state, List<string> path, DfsResult result)
    {
        state[current] = 1;
        path.Add(current);
        result.DiscoveryOrder.Add(current);

        foreach (var next in _adjacency[current])
        {
            if (!state.TryGetValue(next, out var nextState))
            {
                DfsVisit(next, state, path, result);
            }
            else if (nextState == 1)
            {
                // Arista de retroceso: current -> next cierra un ciclo.
                int index = path.IndexOf(next);
                var cycle = path.Skip(index).ToList();
                cycle.Add(next);
                result.Cycles.Add(cycle);
            }
        }

        path.RemoveAt(path.Count - 1);
        state[current] = 2;
        result.FinishOrder.Add(current);
    }

    // ------------------------------------------------------------------
    // Alcanzabilidad
    // ------------------------------------------------------------------

    /// <summary>
    /// Camino más corto (en saltos) de fromId a toId usando BFS.
    /// Devuelve null si no existe camino o si algún vértice no existe.
    /// </summary>
    public List<string>? FindPath(string fromId, string toId)
    {
        var start = Find(fromId);
        var goal = Find(toId);
        if (start == null || goal == null) return null;

        var parent = new Dictionary<string, string?> { [start.Id] = null };
        var queue = new Queue<string>();
        queue.Enqueue(start.Id);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == goal.Id)
                return BuildPath(parent, current);

            foreach (var next in _adjacency[current])
            {
                if (!parent.ContainsKey(next))
                {
                    parent[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }

    public bool IsReachable(string fromId, string toId) => FindPath(fromId, toId) != null;

    private static List<string> BuildPath(Dictionary<string, string?> parent, string end)
    {
        var path = new List<string>();
        string? node = end;
        while (node != null)
        {
            path.Add(node);
            node = parent[node];
        }
        path.Reverse();
        return path;
    }

    // ------------------------------------------------------------------
    // Consultas sociales
    // ------------------------------------------------------------------

    /// <summary>Usuarios con grado de entrada 0 (nadie los sigue).</summary>
    public List<string> GetUsersWithoutFollowers() =>
        _order.Where(id => GetInDegree(id) == 0).ToList();

    /// <summary>Usuarios con el mayor grado de entrada (más seguidores).</summary>
    public List<string> GetMostInfluential() => IdsWithMaxDegree(GetInDegree);

    /// <summary>Usuarios con el mayor grado de salida (siguen a más personas).</summary>
    public List<string> GetMostActive() => IdsWithMaxDegree(GetOutDegree);

    private List<string> IdsWithMaxDegree(Func<string, int> degree)
    {
        if (_order.Count == 0) return new List<string>();
        int max = _order.Max(degree);
        return _order.Where(id => degree(id) == max).ToList();
    }

    // ------------------------------------------------------------------

    private Vertex? Find(string? id) =>
        id != null && _vertices.TryGetValue(id.Trim(), out var vertex) ? vertex : null;
}
