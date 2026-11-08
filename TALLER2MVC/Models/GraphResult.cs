namespace CampusNet.Models;

/// <summary>
/// Resultado de una operación sobre el grafo. El Model no imprime nada:
/// solo informa qué pasó y el Controller decide cómo comunicarlo.
/// </summary>
public enum GraphResult
{
    Ok,
    InvalidData,
    VertexExists,
    VertexNotFound,
    EdgeExists,
    EdgeNotFound,
    SelfLoop
}
