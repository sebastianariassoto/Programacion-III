namespace CampusNet.Models;

/// <summary>Rol académico de un usuario de CampusNet.</summary>
public enum Role
{
    Estudiante,
    Profesor,
    Egresado
}

/// <summary>Vértice del grafo: un usuario de la red.</summary>
public class Vertex
{
    /// <summary>Identificador único (no cambia una vez creado).</summary>
    public string Id { get; }

    public string Name { get; set; }

    public Role Role { get; set; }

    public Vertex(string id, string name, Role role)
    {
        Id = id?.Trim() ?? string.Empty;
        Name = name?.Trim() ?? string.Empty;
        Role = role;
    }

    public override string ToString() => $"{Id} - {Name} ({Role})";
}
