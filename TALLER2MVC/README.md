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














## Conclusiones

- El grafo dirigido permite modelar relaciones **no recíprocas**: seguir a alguien no implica ser seguido.
- La lista de adyacencia es la estructura adecuada para una red dispersa (densidad ≈ 0.17).
- El `Id` determina la identidad del vértice; el nombre y el rol se pueden actualizar sin afectar la estructura.
- BFS resuelve la alcanzabilidad y los caminos más cortos; DFS permite detectar ciclos dirigidos.
- Los usuarios con grado de entrada 0 nunca se alcanzan desde otro usuario: solo se visitan si el recorrido empieza en ellos.
- La arquitectura MVC separa responsabilidades y hace el código más claro y mantenible.
