# Programación-III
## Proyecto de clases

**Integrantes:**

- Sebastián Arias Soto — Desarrollador
- Génesis Rodríguez — Algoritmos & Lógica
- Juan Camilo Toro — Análisis & Pruebas

---

# Proyecto de Programación: Grafos Dirigidos – CampusNet (MVC)

Este proyecto consiste en una implementación en C# de un **grafo dirigido** que modela una red social académica llamada **CampusNet**. Cada usuario puede seguir a otros, pero la relación no es necesariamente recíproca. Incluye operaciones fundamentales como inserción, eliminación, actualización, recorridos en anchura (BFS) y en profundidad (DFS), consultas sociales y métricas de conectividad.

## Características y Funcionalidades

- **Estructura Dinámica:** el grafo se guarda con **lista de adyacencia** mediante `Dictionary<string, List<string>>`, que usa memoria proporcional a vértices + aristas.
- **Operaciones Principales:**
  - **Inserción:** agrega usuarios (vértices) y relaciones de seguimiento (aristas dirigidas) sin permitir duplicados.
  - **Actualización:** modifica el nombre o el rol de un usuario sin alterar su `Id`.
  - **Eliminación:** borra usuarios (junto con todas sus aristas entrantes y salientes) o relaciones individuales.
- **Recorridos:**
  - **BFS (anchura):** visita por capas usando una cola. Sirve para alcanzabilidad y caminos más cortos.
  - **DFS (profundidad):** recorre lo más hondo posible antes de retroceder. Permite detectar ciclos dirigidos.
- **Consultas Sociales:**
  - Usuarios sin seguidores (grado de entrada 0).
  - Usuarios influyentes (mayor grado de entrada).
  - Usuarios más activos (mayor grado de salida).
  - ¿Puede A llegar a B? (BFS con camino).
- **Métricas:** densidad, grado de salida promedio, componentes débil y fuertemente conexas (Kosaraju).

---

## ¿Qué es un grafo dirigido?

Un conjunto de **vértices** unidos por **aristas con dirección**. En CampusNet:

- **Vértice** = un usuario (`Id`, `Name`, `Role`: Estudiante, Profesor o Egresado).
- **Arista `A → B`** = "A sigue a B". No implica que B siga a A.

**Ventaja:** la lista de adyacencia permite recorrer los vecinos de un usuario directamente, sin revisar toda la red.

### Grados

| Concepto | Significado | Qué mide |
|---|---|---|
| Grado de salida de A | a cuántos usuarios sigue A | qué tan **activo** es |
| Grado de entrada de A | cuántos usuarios siguen a A | qué tan **influyente** es |

### Reglas

- No se permiten **vértices duplicados** (el `Id` es único) ni **aristas duplicadas**.
- Un usuario **no puede seguirse a sí mismo**.
- No se permiten Ids ni nombres vacíos.
- Al eliminar un usuario se eliminan también sus relaciones.

---

## Diagrama

Un ciclo dirigido de la red (`U02 → U03 → U04 → U02`):

```
[U02] ──► [U03] ──► [U04]
  ▲                   │
  └───────────────────┘
```

Lista de adyacencia inicial (parcial):

```
U01 (Ana Torres, Profesor)    -> [U02, U03, U04, U05]   salida: 4, entrada: 5
U02 (Bruno Díaz, Estudiante)  -> [U03, U01]             salida: 2, entrada: 3
U05 (Elena Mora, Profesor)    -> [U01, U06, U07, U08, U09]  salida: 5, entrada: 3
U10 (Javier Pardo, Estudiante)-> [U01, U05]             salida: 2, entrada: 0
```

- `[ ]` indica un usuario.
- `→` indica "sigue a".

---

## Recorridos

| Recorrido | Estructura | Para qué sirve |
|---|---|---|
| **BFS** | Cola | Orden por capas, alcanzabilidad y camino más corto |
| **DFS completo** | Recursión (blanco / gris / negro) | Orden de descubrimiento y detección de ciclos |

**Detección de ciclos:** durante el DFS, si desde un nodo se llega a un vecino que sigue "gris" (en el camino actual), esa arista cierra un ciclo.

**BFS e "alcanzados":** solo se visitan los usuarios a los que se puede llegar desde el origen. El conteo incluye el origen.

---

## Eliminar: los casos

| Caso | Situación | Qué ocurre |
|---|---|---|
| 1. Relación | Existe `A → B` | Se quita `B` de la lista de `A` |
| 2. Usuario | Existe el vértice | Se elimina el vértice, su lista y toda arista que lo mencione |
| 3. Inexistente | No existe el vértice o la relación | Se rechaza con un `OperationResult` y no cambia nada |

---

## Arquitectura MVC

```
CampusNet/
├── CampusNet.csproj
├── Program.cs                      # Solo invoca al Controller
├── Models/
│   ├── Graph.cs                    # Lista de adyacencia, CRUD, BFS, DFS, consultas y métricas
│   ├── Vertex.cs
│   ├── Edge.cs
│   ├── Role.cs
│   └── Results.cs                  # BfsResult, DfsResult, DegreeRanking, GraphMetrics, OperationResult
├── Views/
│   └── ConsoleView.cs              # Única clase que usa Console
├── Controllers/
│   └── CampusNetController.cs      # Secuencia de casos de uso
└── docs/
    └── ejecucion.txt               # Salida completa de una ejecución
```

| Capa | Responsabilidad | Restricción |
|---|---|---|
| **Model** | Guarda el grafo y resuelve la lógica | No usa `Console` |
| **View** | Imprime lo que recibe | No modifica el grafo ni decide |
| **Controller** | Orquesta los casos de uso | No imprime directamente |
| **Program** | Punto de entrada | Solo invoca al Controller |

El Model no imprime: devuelve un `OperationResult` (`Ok`, `DuplicateVertex`, `DuplicateEdge`, `VertexNotFound`, `EdgeNotFound`, `SelfLoop`, `InvalidData`) y la View lo traduce a un mensaje.

---

## Complejidad

| Funcionalidad | Complejidad |
|---|---|
| BFS / DFS | O(V + E) |
| Alcanzabilidad A → B | O(V + E) |
| Componentes débil y fuertemente conexas | O(V + E) |
| Grado de salida | O(1) |
| Grado de entrada | O(E) |

---

## Casos de uso que ejecuta el programa

1. **Construcción:** 12 usuarios y 25 relaciones. Cumple los requisitos del taller y los verifica automáticamente:
   - 2 usuarios con grado de salida ≥ 4 (U01 y U05).
   - Ciclos dirigidos.
   - 3 usuarios sin seguidores (U10, U11 y U12).
2. **Recorridos:** 3 BFS (desde U01, U10 y U12) y un DFS completo.
3. **Consultas sociales:** sin seguidores, influyentes, activos y alcanzabilidad.
4. **CRUD:** agregar, actualizar y eliminar usuarios y relaciones, con pruebas de rechazo de duplicados. Tras cada operación se imprimen los vértices afectados.
5. **Análisis final:** estructura final, métricas y justificación de los resultados.

---

## Cómo ejecutar

Requisito: [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior.

```bash
git clone <URL-del-repositorio>
cd <carpeta-donde-esta-CampusNet.csproj>
dotnet run
```

En Visual Studio: abrir el archivo `CampusNet.csproj` (no los `.cs` sueltos) y ejecutar con `Ctrl + F5`.

La salida completa de una ejecución está en [`docs/ejecucion.txt`](docs/ejecucion.txt).

---

## Checklist de pruebas mínimas

- [x] Grafo dirigido con ≥ 12 vértices y ≥ 18 aristas.
- [x] Impresión de la lista de adyacencia.
- [x] 3 BFS y 1 DFS completos.
- [x] Consultas de usuarios influyentes, activos y sin seguidores.
- [x] Al menos 1 ciclo dirigido detectable.
- [x] 3 operaciones CRUD funcionales (agregar, eliminar y actualizar).

---

## Evidencia de ejecución

_Agregar aquí capturas de la consola._

<!-- Ejemplo: ![Construcción del grafo](docs/captura1.png) -->

---

## Conclusiones

- El grafo dirigido permite modelar relaciones **no recíprocas**: seguir a alguien no implica ser seguido.
- La lista de adyacencia es la estructura adecuada para una red dispersa (densidad ≈ 0.17).
- El `Id` determina la identidad del vértice; el nombre y el rol se pueden actualizar sin afectar la estructura.
- BFS resuelve la alcanzabilidad y los caminos más cortos; DFS permite detectar ciclos dirigidos.
- Los usuarios con grado de entrada 0 nunca se alcanzan desde otro usuario: solo se visitan si el recorrido empieza en ellos.
- La arquitectura MVC separa responsabilidades y hace el código más claro y mantenible.
