# Programación-III
proyecto de clases 


Integrantes: 
* **Sebastián Arias Soto** — *Desarrollador*
* **Génesis Rodríguez** — *Algoritmos & Lógica*
* **Juan Camilo Toro** — *Análisis & Pruebas*

Proyecto de Programación: Árboles Binarios de Búsqueda (MVC)

Este proyecto consiste en una implementación en C# de una estructura de datos de **Árbol Binario (MVC)**. Incluye operaciones fundamentales como inserción, búsqueda, eliminación, recorridos recursivos y análisis de complejidad algorítmica.

Características y Funcionalidades

- **Estructura Dinámica:** Manejo eficiente de memoria dinámica mediante punteros (`Node*`).
- **Operaciones Principales:**
  - **Inserción:** Ubicación organizada de elementos según la propiedad BST ($izq < raíz < der$).
  - **Búsqueda Optimizada:** Localización de elementos en tiempo logarítmico.
  - **Eliminación:** Reestructuración de nodos mediante el cálculo del sucesor inorden.
- **Recorridos Recursivos:**
  - `Inorden`: Devuelve los elementos ordenados de forma ascendente.
  - `Preorden`: Útil para clonación y serialización de estructuras.
  - `Postorden`: Utilizado para la liberación secuencial de memoria.
 
¿Qué es un BST?

Un árbol donde cada nodo tiene **máximo dos hijos** y cumple una regla:

> **Izquierda = menores · Derecha = mayores**

```
            Proyectos
           /         \
      Backups         Tareas
      /     \              \
  Anexos  Contratos.pdf    Videos
```

**Ventaja:** para buscar, insertar o eliminar solo se recorre **un camino**, no todo el árbol.


**Reglas:**
1. No se permiten **duplicados** ni nombres vacíos.
2. Se baja desde la raíz comparando en cada nodo.
3. El nodo se crea en el **primer hueco** (`null`) que se encuentre.

**Ejemplo:** insertar `Fotos` → `Proyectos` (izq) → `Backups` (der) → `Contratos.pdf` (der) → hueco → **se crea ahí**.

Eliminar: Los 3 casos

| Caso | Situación | Qué ocurre |
|---|---|---|
| **1. Hoja** | Sin hijos | El nodo simplemente desaparece |
| **2. Un hijo** | Un solo hijo | El hijo **ocupa su lugar** |
| **3. Dos hijos** | Ambos hijos | Se reemplaza por el **sucesor inorden** |

**Sucesor inorden** = el menor del subárbol derecho (el más a la izquierda). Se copia su dato al nodo y luego se elimina el sucesor.

Diagrama ASCII

El árbol se dibuja "acostado": los **derechos arriba** y los **izquierdos abajo**.

```
            /-- [Videos]
      /-- [Tareas]
[Proyectos]
            /-- Contratos.pdf
      \-- [Backups]
            \-- [Anexos]
```

- `[ ]` indica una **carpeta**.
- `/--` hijo derecho · `\--` hijo izquierdo.
- Se construye con recursión y un `StringBuilder`.


Conclusiones

- El BST permite **buscar, insertar y eliminar** recorriendo solo un camino.
- La **clave** (nombre) determina la estructura; los demás datos no la afectan.
- Eliminar tiene **3 casos**, y el de dos hijos se resuelve con el **sucesor inorden**.
- Los **recorridos** ofrecen distintas vistas de los mismos datos; el inorden da el orden alfabético.
- La arquitectura **MVC** separa responsabilidades y hace el código más claro y mantenible.

---
