using CampusNet.Models;
using CampusNet.Views;

namespace CampusNet.Controllers;

/// <summary>
/// Controller: ejecuta la secuencia de casos de uso y comunica Model y View.
/// </summary>
public class CampusNetController
{
    private readonly Graph _graph = new();
    private readonly ConsoleView _view = new();

    // Datos iniciales: 12 usuarios con roles variados.
    private static readonly (string Id, string Name, Role Role)[] InitialUsers =
    {
        ("U01", "Ana Torres",     Role.Estudiante),
        ("U02", "Carlos Ruiz",    Role.Profesor),
        ("U03", "Laura Gómez",    Role.Estudiante),
        ("U04", "Miguel Herrera", Role.Egresado),
        ("U05", "Sofía Ramírez",  Role.Profesor),
        ("U06", "Diego Castaño",  Role.Estudiante),
        ("U07", "Valentina Ríos", Role.Egresado),
        ("U08", "Andrés Mejía",   Role.Estudiante),
        ("U09", "Camila Duque",   Role.Profesor),
        ("U10", "Julián Osorio",  Role.Estudiante),
        ("U11", "Paula Vargas",   Role.Egresado),
        ("U12", "Felipe Cardona", Role.Estudiante)
    };

    // Datos iniciales: 22 relaciones (A -> B significa que A sigue a B).
    private static readonly (string Source, string Target)[] InitialRelations =
    {
        ("U01", "U02"), ("U01", "U03"), ("U01", "U05"), ("U01", "U09"),
        ("U02", "U05"),
        ("U03", "U04"),
        ("U04", "U05"),
        ("U05", "U09"),
        ("U06", "U02"), ("U06", "U03"),
        ("U07", "U05"), ("U07", "U09"),
        ("U08", "U02"), ("U08", "U04"), ("U08", "U05"), ("U08", "U09"),
        ("U09", "U02"), ("U09", "U07"),
        ("U10", "U06"), ("U10", "U02"),
        ("U11", "U05"),
        ("U12", "U08")
    };

    public void Run()
    {
        _view.ShowTitle("CAMPUSNET - Red social dirigida de usuarios académicos");

        BuildGraph();
        RunTraversals();
        RunSocialQueries();
        RunCrudOperations();
        ShowFinalState();
    }

    // ==================================================================
    // 1. Construcción del grafo
    // ==================================================================

    private void BuildGraph()
    {
        _view.ShowTitle("1. Construcción del grafo");
        _view.ShowMessage(
            $"Grafo dirigido vacío creado: {_graph.VertexCount} vértices, {_graph.EdgeCount} aristas.");

        _view.ShowSection("Inserción de usuarios");
        foreach (var (id, name, role) in InitialUsers)
            AddUser(id, name, role);

        _view.ShowSection("Inserción de relaciones (A -> B: A sigue a B)");
        foreach (var (source, target) in InitialRelations)
            Follow(source, target);

        _view.ShowSection("Estructura del grafo (lista de adyacencias)");
        _view.ShowAdjacencyList(_graph);

        _view.ShowSection("Verificación de requisitos mínimos");
        VerifyRequirements();
    }

    private void VerifyRequirements()
    {
        int highOutDegree = _graph.VertexIds.Count(id => _graph.GetOutDegree(id) >= 4);
        int withoutFollowers = _graph.GetUsersWithoutFollowers().Count;
        var dfs = _graph.Dfs();
        int distinctRoles = _graph.GetVertices().Select(v => v.Role).Distinct().Count();

        _view.ShowChecklist(new List<(string Description, bool Passed)>
        {
            ($"Al menos 12 usuarios (actual: {_graph.VertexCount})", _graph.VertexCount >= 12),
            ($"Al menos 18 relaciones (actual: {_graph.EdgeCount})", _graph.EdgeCount >= 18),
            ($"Al menos 2 usuarios con grado de salida >= 4 (actual: {highOutDegree})", highOutDegree >= 2),
            ($"Al menos 1 ciclo dirigido (detectados: {dfs.Cycles.Count})", dfs.HasCycle),
            ($"Al menos 2 usuarios sin seguidores (actual: {withoutFollowers})", withoutFollowers >= 2),
            ($"Roles variados (distintos: {distinctRoles} de 3)", distinctRoles == 3)
        });
    }

    // ==================================================================
    // 2. Recorridos
    // ==================================================================

    private void RunTraversals()
    {
        _view.ShowTitle("2. Recorridos");

        _view.ShowSection("BFS desde tres usuarios distintos");
        foreach (var start in new[] { "U01", "U10", "U12" })
        {
            var order = _graph.Bfs(start);
            _view.ShowBfs(_graph, start, order);
        }

        _view.ShowSection("DFS completo");
        _view.ShowDfs(_graph.Dfs());
    }

    // ==================================================================
    // 3. Consultas sociales
    // ==================================================================

    private void RunSocialQueries()
    {
        _view.ShowTitle("3. Consultas sociales");

        _view.ShowSection("Usuarios sin seguidores (grado de entrada 0)");
        _view.ShowUsers(_graph, _graph.GetUsersWithoutFollowers());

        _view.ShowSection("Usuarios más influyentes (mayor grado de entrada)");
        _view.ShowUsers(_graph, _graph.GetMostInfluential());

        _view.ShowSection("Usuarios más activos (mayor grado de salida)");
        _view.ShowUsers(_graph, _graph.GetMostActive());

        _view.ShowSection("Alcanzabilidad mediante BFS");
        foreach (var (source, target) in new[] { ("U12", "U07"), ("U10", "U03"), ("U05", "U01") })
            _view.ShowReachability(_graph, source, target, _graph.FindPath(source, target));
    }

    // ==================================================================
    // 4. Operaciones CRUD (cada una imprime sus cambios)
    // ==================================================================

    private void RunCrudOperations()
    {
        _view.ShowTitle("4. Operaciones CRUD");

        _view.ShowSection("Agregar usuario");
        AddUser("U13", "Marta Salazar", Role.Estudiante);

        _view.ShowSection("Control de duplicados y datos inválidos");
        AddUser("U13", "Marta Duplicada", Role.Profesor);   // vértice duplicado
        Follow("U01", "U02");                               // arista duplicada
        Follow("U13", "U13");                               // seguirse a sí mismo
        Follow("U13", "U99");                               // usuario inexistente

        _view.ShowSection("Agregar relaciones");
        Follow("U13", "U05");
        Follow("U12", "U13");

        _view.ShowSection("Actualizar nombre y rol");
        RenameUser("U03", "Laura Gómez Pérez");
        ChangeRole("U06", Role.Egresado);

        _view.ShowSection("Eliminar relación");
        Unfollow("U01", "U03");

        _view.ShowSection("Eliminar usuario (también elimina sus relaciones)");
        DeleteUser("U04");
    }

    private void ShowFinalState()
    {
        _view.ShowTitle("5. Estado final del grafo");
        _view.ShowAdjacencyList(_graph);

        _view.ShowSection("DFS completo sobre el grafo final");
        _view.ShowDfs(_graph.Dfs());
    }

    // ==================================================================
    // Operaciones auxiliares: llaman al Model e informan con la View
    // ==================================================================

    private void AddUser(string id, string name, Role role)
    {
        var vertex = new Vertex(id, name, role);
        var result = _graph.AddVertex(vertex);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Usuario agregado: {vertex}");
        }
        else
        {
            _view.ShowError($"No se pudo agregar el usuario '{id}': {Describe(result)}");
        }
    }

    private void Follow(string sourceId, string targetId)
    {
        var result = _graph.AddEdge(sourceId, targetId);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Relación agregada: {Label(sourceId)} ahora sigue a {Label(targetId)}");
        }
        else
        {
            _view.ShowError($"No se pudo agregar {sourceId} -> {targetId}: {Describe(result)}");
        }
    }

    private void Unfollow(string sourceId, string targetId)
    {
        string sourceLabel = Label(sourceId);
        string targetLabel = Label(targetId);
        var result = _graph.RemoveEdge(sourceId, targetId);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Relación eliminada: {sourceLabel} dejó de seguir a {targetLabel}");
        }
        else
        {
            _view.ShowError($"No se pudo eliminar {sourceId} -> {targetId}: {Describe(result)}");
        }
    }

    private void DeleteUser(string id)
    {
        var vertex = _graph.GetVertex(id);
        var incidentEdges = _graph.GetIncidentEdges(id);
        var result = _graph.RemoveVertex(id);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Usuario eliminado: {vertex}");
            if (incidentEdges.Count > 0)
            {
                _view.ShowMessage(
                    $"          Relaciones eliminadas en cascada ({incidentEdges.Count}): " +
                    string.Join(", ", incidentEdges));
            }
        }
        else
        {
            _view.ShowError($"No se pudo eliminar el usuario '{id}': {Describe(result)}");
        }
    }

    private void RenameUser(string id, string newName)
    {
        string oldName = _graph.GetVertex(id)?.Name ?? string.Empty;
        var result = _graph.UpdateName(id, newName);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Nombre actualizado en {id}: '{oldName}' -> '{newName}'");
            _view.ShowVertexRow(_graph, id);
        }
        else
        {
            _view.ShowError($"No se pudo actualizar el nombre de '{id}': {Describe(result)}");
        }
    }

    private void ChangeRole(string id, Role newRole)
    {
        var oldRole = _graph.GetVertex(id)?.Role;
        var result = _graph.UpdateRole(id, newRole);

        if (result == GraphResult.Ok)
        {
            _view.ShowSuccess($"Rol actualizado en {id}: {oldRole} -> {newRole}");
            _view.ShowVertexRow(_graph, id);
        }
        else
        {
            _view.ShowError($"No se pudo actualizar el rol de '{id}': {Describe(result)}");
        }
    }

    private string Label(string id)
    {
        var vertex = _graph.GetVertex(id);
        return vertex == null ? id : $"{vertex.Id} ({vertex.Name})";
    }

    private static string Describe(GraphResult result) => result switch
    {
        GraphResult.Ok => "operación exitosa",
        GraphResult.InvalidData => "datos inválidos (id o nombre vacío)",
        GraphResult.VertexExists => "el usuario ya existe (duplicado)",
        GraphResult.VertexNotFound => "el usuario no existe",
        GraphResult.EdgeExists => "la relación ya existe (duplicado)",
        GraphResult.EdgeNotFound => "la relación no existe",
        GraphResult.SelfLoop => "un usuario no puede seguirse a sí mismo",
        _ => "error desconocido"
    };
}
