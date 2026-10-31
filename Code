using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TallerMVC.Controllers;
using TallerMVC.Models;
using TallerMVC.Views;

// ============================================================================
// TallerMVC - Taller Integrador BST (C#, arquitectura MVC)
// Todo en un solo archivo, pero separado en 3 capas (namespaces):
//   TallerMVC.Models      -> lógica del árbol (sin Console)
//   TallerMVC.Views       -> entrada/salida por consola
//   TallerMVC.Controllers -> conecta vista y modelo
// ============================================================================

// ---------------------------- MODELO: Documento ----------------------------
namespace TallerMVC.Models
{
    /// <summary>
    /// Dato que guarda cada nodo del árbol: puede ser una carpeta o un archivo.
    /// La CLAVE de ordenamiento del BST es el Nombre.
    /// </summary>
    public class Documento
    {
        public string Nombre { get; set; }
        public bool EsCarpeta { get; set; }
        public long TamanoKb { get; set; }
        public DateTime FechaModificacion { get; set; }

        public Documento(string nombre, bool esCarpeta, long tamanoKb = 0)
        {
            Nombre = nombre;
            EsCarpeta = esCarpeta;
            TamanoKb = esCarpeta ? 0 : tamanoKb;
            FechaModificacion = DateTime.Now;
        }

        public override string ToString()
        {
            string tipo = EsCarpeta ? "Carpeta" : "Archivo";
            return $"{Nombre} | {tipo} | {TamanoKb} KB | {FechaModificacion:yyyy-MM-dd HH:mm}";
        }
    }
}

// ---------------------------- MODELO: NodoBST ----------------------------
namespace TallerMVC.Models
{
    /// <summary>Nodo del árbol: guarda un Documento y apunta a sus dos hijos.</summary>
    public class NodoBST
    {
        public Documento Dato { get; set; }
        public NodoBST Izq { get; set; }
        public NodoBST Der { get; set; }

        public NodoBST(Documento dato)
        {
            Dato = dato;
        }

        public bool EsHoja => Izq == null && Der == null;
    }
}

// ---------------------------- MODELO: ArbolBST (CRUD, recorridos, métricas, diagrama) ----------------------------
namespace TallerMVC.Models
{
    /// <summary>
    /// Árbol Binario de Búsqueda de documentos, ordenado por nombre
    /// (sin distinguir mayúsculas/minúsculas). No usa Console: solo lógica.
    /// </summary>
    public class ArbolBST
    {
        public NodoBST Raiz { get; private set; }

        private static int Comparar(string a, string b) =>
            string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

        // =====================================================================
        // C - CREATE
        // =====================================================================
        /// <summary>Inserta un documento. Devuelve false si el nombre ya existe.</summary>
        public bool Insertar(Documento doc)
        {
            if (doc == null || string.IsNullOrWhiteSpace(doc.Nombre)) return false;
            if (Buscar(doc.Nombre) != null) return false;   // no se permiten duplicados
            Raiz = Insertar(Raiz, doc);
            return true;
        }

        private NodoBST Insertar(NodoBST nodo, Documento doc)
        {
            if (nodo == null) return new NodoBST(doc);          // caso base: hueco encontrado
            if (Comparar(doc.Nombre, nodo.Dato.Nombre) < 0)
                nodo.Izq = Insertar(nodo.Izq, doc);             // menor -> izquierda
            else
                nodo.Der = Insertar(nodo.Der, doc);             // mayor -> derecha
            return nodo;
        }

        // =====================================================================
        // R - READ (búsquedas)
        // =====================================================================
        /// <summary>Búsqueda iterativa por nombre. Devuelve null si no existe.</summary>
        public Documento Buscar(string nombre) => BuscarNodo(nombre)?.Dato;

        private NodoBST BuscarNodo(string nombre)
        {
            NodoBST actual = Raiz;
            while (actual != null)
            {
                int c = Comparar(nombre, actual.Dato.Nombre);
                if (c == 0) return actual;
                actual = c < 0 ? actual.Izq : actual.Der;
            }
            return null;
        }

        /// <summary>
        /// Igual que Buscar, pero devuelve el camino recorrido (sirve para demostrar
        /// que la búsqueda es rápida: solo visita un camino, no todo el árbol).
        /// </summary>
        public Documento BuscarConCamino(string nombre, out List<string> camino)
        {
            camino = new List<string>();
            NodoBST actual = Raiz;
            while (actual != null)
            {
                camino.Add(actual.Dato.Nombre);
                int c = Comparar(nombre, actual.Dato.Nombre);
                if (c == 0) return actual.Dato;
                actual = c < 0 ? actual.Izq : actual.Der;
            }
            return null;
        }

        /// <summary>Todos los documentos cuyo nombre empieza por el prefijo.</summary>
        public List<Documento> BuscarPorPrefijo(string prefijo)
        {
            var resultado = new List<Documento>();
            BuscarPorPrefijo(Raiz, prefijo, resultado);
            return resultado;
        }

        private void BuscarPorPrefijo(NodoBST nodo, string prefijo, List<Documento> res)
        {
            if (nodo == null) return;
            BuscarPorPrefijo(nodo.Izq, prefijo, res);
            if (nodo.Dato.Nombre.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                res.Add(nodo.Dato);
            BuscarPorPrefijo(nodo.Der, prefijo, res);
        }

        // =====================================================================
        // U - UPDATE
        // =====================================================================
        /// <summary>Actualiza el tamaño y la fecha. La clave (nombre) no cambia,
        /// por eso el nodo NO se mueve de lugar en el árbol.</summary>
        public bool Actualizar(string nombre, long nuevoTamanoKb)
        {
            Documento doc = Buscar(nombre);
            if (doc == null) return false;
            doc.TamanoKb = nuevoTamanoKb;
            doc.FechaModificacion = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Cambiar el nombre SÍ cambia la clave, así que se elimina y se vuelve a
        /// insertar para que el nodo quede en la posición correcta.
        /// </summary>
        public bool Renombrar(string nombreActual, string nombreNuevo)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevo)) return false;
            if (Buscar(nombreNuevo) != null) return false;     // el nuevo nombre ya existe
            Documento doc = Buscar(nombreActual);
            if (doc == null) return false;
            Eliminar(nombreActual, out _);
            doc.Nombre = nombreNuevo;
            doc.FechaModificacion = DateTime.Now;
            return Insertar(doc);
        }

        // =====================================================================
        // D - DELETE (3 casos)
        // =====================================================================
        /// <summary>
        /// Elimina por nombre. 'caso' indica cuál de los 3 casos se aplicó:
        /// "Hoja", "Un hijo" o "Dos hijos (sucesor inorden)".
        /// </summary>
        public bool Eliminar(string nombre, out string caso)
        {
            caso = null;
            NodoBST nodo = BuscarNodo(nombre);
            if (nodo == null) return false;

            if (nodo.EsHoja) caso = "Hoja";
            else if (nodo.Izq == null || nodo.Der == null) caso = "Un hijo";
            else caso = "Dos hijos (sucesor inorden)";

            Raiz = Eliminar(Raiz, nombre);
            return true;
        }

        private NodoBST Eliminar(NodoBST nodo, string nombre)
        {
            if (nodo == null) return null;

            int c = Comparar(nombre, nodo.Dato.Nombre);
            if (c < 0) nodo.Izq = Eliminar(nodo.Izq, nombre);
            else if (c > 0) nodo.Der = Eliminar(nodo.Der, nombre);
            else
            {
                // Caso 1 (hoja) y Caso 2 (un hijo): el hijo existente (o null) toma su lugar
                if (nodo.Izq == null) return nodo.Der;
                if (nodo.Der == null) return nodo.Izq;

                // Caso 3 (dos hijos): se reemplaza por el SUCESOR INORDEN
                // (el menor del subárbol derecho) y luego se borra ese sucesor.
                NodoBST sucesor = Minimo(nodo.Der);
                nodo.Dato = sucesor.Dato;
                nodo.Der = Eliminar(nodo.Der, sucesor.Dato.Nombre);
            }
            return nodo;
        }

        private static NodoBST Minimo(NodoBST nodo)
        {
            while (nodo.Izq != null) nodo = nodo.Izq;
            return nodo;
        }

        // =====================================================================
        // RECORRIDOS
        // =====================================================================
        /// <summary>Izquierda - Raíz - Derecha: sale ordenado alfabéticamente.</summary>
        public List<Documento> InOrden()
        {
            var lista = new List<Documento>();
            InOrden(Raiz, lista);
            return lista;
        }

        private void InOrden(NodoBST n, List<Documento> l)
        {
            if (n == null) return;
            InOrden(n.Izq, l);
            l.Add(n.Dato);
            InOrden(n.Der, l);
        }

        /// <summary>Raíz - Izquierda - Derecha: útil para copiar el árbol.</summary>
        public List<Documento> PreOrden()
        {
            var lista = new List<Documento>();
            PreOrden(Raiz, lista);
            return lista;
        }

        private void PreOrden(NodoBST n, List<Documento> l)
        {
            if (n == null) return;
            l.Add(n.Dato);
            PreOrden(n.Izq, l);
            PreOrden(n.Der, l);
        }

        /// <summary>Izquierda - Derecha - Raíz: útil para borrar el árbol.</summary>
        public List<Documento> PostOrden()
        {
            var lista = new List<Documento>();
            PostOrden(Raiz, lista);
            return lista;
        }

        private void PostOrden(NodoBST n, List<Documento> l)
        {
            if (n == null) return;
            PostOrden(n.Izq, l);
            PostOrden(n.Der, l);
            l.Add(n.Dato);
        }

        /// <summary>Por niveles (anchura), usando una cola.</summary>
        public List<Documento> PorNiveles()
        {
            var lista = new List<Documento>();
            if (Raiz == null) return lista;
            var cola = new Queue<NodoBST>();
            cola.Enqueue(Raiz);
            while (cola.Count > 0)
            {
                NodoBST n = cola.Dequeue();
                lista.Add(n.Dato);
                if (n.Izq != null) cola.Enqueue(n.Izq);
                if (n.Der != null) cola.Enqueue(n.Der);
            }
            return lista;
        }

        // =====================================================================
        // MÉTRICAS Y MÉTODOS AUXILIARES
        // =====================================================================
        public bool EstaVacio => Raiz == null;

        /// <summary>Altura en niveles: árbol vacío = 0, solo raíz = 1.</summary>
        public int Altura() => Altura(Raiz);
        private int Altura(NodoBST n) => n == null ? 0 : 1 + Math.Max(Altura(n.Izq), Altura(n.Der));

        public int ContarNodos() => ContarNodos(Raiz);
        private int ContarNodos(NodoBST n) => n == null ? 0 : 1 + ContarNodos(n.Izq) + ContarNodos(n.Der);

        public int ContarHojas() => ContarHojas(Raiz);

        private int ContarHojas(NodoBST n)
        {
            if (n == null) return 0;
            if (n.EsHoja) return 1;
            return ContarHojas(n.Izq) + ContarHojas(n.Der);
        }

        public int ContarCarpetas() => InOrden().Count(d => d.EsCarpeta);
        public int ContarArchivos() => InOrden().Count(d => !d.EsCarpeta);
        public long TamanoTotalKb() => InOrden().Sum(d => d.TamanoKb);

        public Documento Menor() => Raiz == null ? null : Minimo(Raiz).Dato;

        public Documento Mayor()
        {
            NodoBST n = Raiz;
            while (n?.Der != null) n = n.Der;
            return n?.Dato;
        }

        /// <summary>Nivel (profundidad) de un nodo: raíz = 1. Devuelve 0 si no existe.</summary>
        public int NivelDe(string nombre)
        {
            int nivel = 1;
            NodoBST actual = Raiz;
            while (actual != null)
            {
                int c = Comparar(nombre, actual.Dato.Nombre);
                if (c == 0) return nivel;
                actual = c < 0 ? actual.Izq : actual.Der;
                nivel++;
            }
            return 0;
        }

        // =====================================================================
        // DIAGRAMA ASCII
        // =====================================================================
        /// <summary>
        /// Árbol "acostado": la raíz queda a la izquierda, los hijos DERECHOS
        /// se dibujan arriba (/--) y los IZQUIERDOS abajo (\--).
        /// Las carpetas se muestran entre corchetes [Carpeta].
        /// </summary>
        public string DiagramaAscii()
        {
            if (Raiz == null) return "(árbol vacío)";
            var sb = new StringBuilder();
            Dibujar(Raiz, 0, ' ', sb);
            return sb.ToString().TrimEnd();
        }

        private void Dibujar(NodoBST nodo, int nivel, char lado, StringBuilder sb)
        {
            if (nodo == null) return;

            Dibujar(nodo.Der, nivel + 1, 'D', sb);              // 1º derecha (queda arriba)

            string sangria = new string(' ', nivel * 6);
            string conector = lado == 'D' ? "/-- " : lado == 'I' ? "\\-- " : "";
            string etiqueta = nodo.Dato.EsCarpeta ? $"[{nodo.Dato.Nombre}]" : nodo.Dato.Nombre;
            sb.AppendLine(sangria + conector + etiqueta);       // 2º el nodo

            Dibujar(nodo.Izq, nivel + 1, 'I', sb);              // 3º izquierda (queda abajo)
        }
    }
}

// ---------------------------- VISTA: ConsolaView ----------------------------
namespace TallerMVC.Views
{
    /// <summary>
    /// VISTA: se encarga únicamente de mostrar información y leer datos del usuario.
    /// No conoce las reglas del árbol (eso es del Modelo).
    /// </summary>
    public class ConsolaView
    {
        public int MostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=========== TALLER MVC (BST) ===========");
            Console.WriteLine(" 1. Insertar documento");
            Console.WriteLine(" 2. Buscar documento (con camino)");
            Console.WriteLine(" 3. Buscar por prefijo");
            Console.WriteLine(" 4. Actualizar tamaño");
            Console.WriteLine(" 5. Renombrar documento");
            Console.WriteLine(" 6. Eliminar documento");
            Console.WriteLine(" 7. Recorridos (In/Pre/Post/Niveles)");
            Console.WriteLine(" 8. Diagrama ASCII del árbol");
            Console.WriteLine(" 9. Métricas y altura");
            Console.WriteLine("10. Ejecutar escenarios de la actividad");
            Console.WriteLine("11. Recargar árbol de ejemplo");
            Console.WriteLine(" 0. Salir");
            return PedirEntero("Opción: ");
        }

        // ---------------------- ENTRADA ----------------------
        public string PedirTexto(string mensaje)
        {
            Console.Write(mensaje);
            return (Console.ReadLine() ?? "").Trim();
        }

        public int PedirEntero(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out int valor)) return valor;
                Console.WriteLine("  Ingresa un número entero válido.");
            }
        }

        public bool PedirSiNo(string mensaje)
        {
            Console.Write(mensaje + " (s/n): ");
            return (Console.ReadLine() ?? "").Trim().ToLower() == "s";
        }

        public Documento PedirDocumento()
        {
            string nombre = PedirTexto("Nombre: ");
            bool esCarpeta = PedirSiNo("¿Es carpeta?");
            long tamano = esCarpeta ? 0 : PedirEntero("Tamaño (KB): ");
            return new Documento(nombre, esCarpeta, tamano);
        }

        // ---------------------- SALIDA ----------------------
        public void Mensaje(string texto) => Console.WriteLine(texto);

        public void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {texto} ---");
        }

        public void MostrarDocumento(Documento doc)
        {
            Console.WriteLine(doc != null ? doc.ToString() : "No se encontró el documento.");
        }

        public void MostrarCamino(List<string> camino)
        {
            Console.WriteLine($"Camino recorrido ({camino.Count} comparaciones): " +
                              string.Join(" -> ", camino));
        }

        public void MostrarLista(string titulo, List<Documento> documentos)
        {
            Titulo(titulo);
            if (documentos.Count == 0) { Console.WriteLine("(vacío)"); return; }
            Console.WriteLine(string.Join(", ", documentos.Select(d => d.Nombre)));
        }

        public void MostrarListaDetallada(string titulo, List<Documento> documentos)
        {
            Titulo(titulo);
            if (documentos.Count == 0) Console.WriteLine("(sin resultados)");
            foreach (var d in documentos) Console.WriteLine(d);
        }

        public void MostrarDiagrama(string diagrama)
        {
            Titulo("DIAGRAMA DEL ÁRBOL  (arriba = derechos, abajo = izquierdos, [ ] = carpeta)");
            Console.WriteLine(diagrama);
        }

        public void MostrarMetricas(int altura, int nodos, int hojas, int carpetas,
                                    int archivos, long tamanoTotal, Documento menor, Documento mayor)
        {
            Titulo("MÉTRICAS");
            Console.WriteLine($"Altura (niveles)     : {altura}");
            Console.WriteLine($"Total de nodos       : {nodos}");
            Console.WriteLine($"Nodos hoja           : {hojas}");
            Console.WriteLine($"Carpetas / Archivos  : {carpetas} / {archivos}");
            Console.WriteLine($"Tamaño total         : {tamanoTotal} KB");
            Console.WriteLine($"Menor (alfabético)   : {(menor != null ? menor.Nombre : "-")}");
            Console.WriteLine($"Mayor (alfabético)   : {(mayor != null ? mayor.Nombre : "-")}");
        }
    }
}

// ---------------------------- CONTROLADOR: DocumentoController ----------------------------
namespace TallerMVC.Controllers
{
    /// <summary>
    /// CONTROLADOR: recibe las opciones de la Vista, llama al Modelo y
    /// devuelve los resultados a la Vista.
    /// </summary>
    public class DocumentoController
    {
        private ArbolBST _arbol = new ArbolBST();
        private readonly ConsolaView _vista = new ConsolaView();

        public void Iniciar()
        {
            CargarEjemplo();
            _vista.Mensaje("Árbol de ejemplo cargado.");
            _vista.MostrarDiagrama(_arbol.DiagramaAscii());

            int opcion;
            do
            {
                opcion = _vista.MostrarMenu();
                switch (opcion)
                {
                    case 1: Insertar(); break;
                    case 2: Buscar(); break;
                    case 3: BuscarPrefijo(); break;
                    case 4: Actualizar(); break;
                    case 5: Renombrar(); break;
                    case 6: Eliminar(); break;
                    case 7: MostrarRecorridos(); break;
                    case 8: _vista.MostrarDiagrama(_arbol.DiagramaAscii()); break;
                    case 9: MostrarMetricas(); break;
                    case 10: EjecutarEscenarios(); break;
                    case 11:
                        CargarEjemplo();
                        _vista.Mensaje("Árbol de ejemplo recargado.");
                        _vista.MostrarDiagrama(_arbol.DiagramaAscii());
                        break;
                    case 0: _vista.Mensaje("Hasta luego."); break;
                    default: _vista.Mensaje("Opción inválida."); break;
                }
            } while (opcion != 0);
        }

        // ------------------------------------------------------------------
        // Datos de ejemplo. Forma del árbol resultante:
        //
        //                 Proyectos          <- raíz (2 hijos)
        //                /          \
        //         Backups            Tareas  <- Tareas tiene 1 solo hijo
        //         /     \                \
        //     Anexos  Contratos          Videos   <- Anexos, Contratos y Videos son hojas
        // ------------------------------------------------------------------
        private void CargarEjemplo()
        {
            _arbol = new ArbolBST();
            _arbol.Insertar(new Documento("Proyectos", true));
            _arbol.Insertar(new Documento("Backups", true));
            _arbol.Insertar(new Documento("Tareas", true));
            _arbol.Insertar(new Documento("Anexos", true));
            _arbol.Insertar(new Documento("Contratos.pdf", false, 340));
            _arbol.Insertar(new Documento("Videos", true));
        }

        // ------------------------------ CRUD ------------------------------
        private void Insertar()
        {
            Documento doc = _vista.PedirDocumento();
            _vista.Mensaje(_arbol.Insertar(doc)
                ? "Documento insertado."
                : "No se pudo insertar (nombre vacío o ya existe).");
        }

        private void Buscar()
        {
            string nombre = _vista.PedirTexto("Nombre a buscar: ");
            Documento doc = _arbol.BuscarConCamino(nombre, out List<string> camino);
            _vista.MostrarDocumento(doc);
            _vista.MostrarCamino(camino);
            if (doc != null)
                _vista.Mensaje($"Nivel en el árbol: {_arbol.NivelDe(nombre)}");
        }

        private void BuscarPrefijo()
        {
            string prefijo = _vista.PedirTexto("Prefijo: ");
            _vista.MostrarListaDetallada($"Resultados para '{prefijo}'", _arbol.BuscarPorPrefijo(prefijo));
        }

        private void Actualizar()
        {
            string nombre = _vista.PedirTexto("Nombre del documento: ");
            long tamano = _vista.PedirEntero("Nuevo tamaño (KB): ");
            _vista.Mensaje(_arbol.Actualizar(nombre, tamano)
                ? "Documento actualizado."
                : "No se encontró el documento.");
        }

        private void Renombrar()
        {
            string actual = _vista.PedirTexto("Nombre actual: ");
            string nuevo = _vista.PedirTexto("Nombre nuevo: ");
            _vista.Mensaje(_arbol.Renombrar(actual, nuevo)
                ? "Documento renombrado (se reubicó en el árbol)."
                : "No se pudo renombrar (no existe o el nuevo nombre ya está en uso).");
        }

        private void Eliminar()
        {
            string nombre = _vista.PedirTexto("Nombre a eliminar: ");
            if (_arbol.Eliminar(nombre, out string caso))
            {
                _vista.Mensaje($"Eliminado. Caso aplicado: {caso}");
                _vista.MostrarDiagrama(_arbol.DiagramaAscii());
            }
            else
            {
                _vista.Mensaje("No se encontró el documento.");
            }
        }

        // ----------------------- Recorridos y métricas -----------------------
        private void MostrarRecorridos()
        {
            _vista.MostrarLista("InOrden (Izq-Raíz-Der)", _arbol.InOrden());
            _vista.MostrarLista("PreOrden (Raíz-Izq-Der)", _arbol.PreOrden());
            _vista.MostrarLista("PostOrden (Izq-Der-Raíz)", _arbol.PostOrden());
            _vista.MostrarLista("Por niveles", _arbol.PorNiveles());
        }

        private void MostrarMetricas()
        {
            _vista.MostrarMetricas(
                _arbol.Altura(), _arbol.ContarNodos(), _arbol.ContarHojas(),
                _arbol.ContarCarpetas(), _arbol.ContarArchivos(), _arbol.TamanoTotalKb(),
                _arbol.Menor(), _arbol.Mayor());
        }

        // ------------------ Escenarios exigidos por la actividad ------------------
        private void EjecutarEscenarios()
        {
            CargarEjemplo();
            _vista.Titulo("ESCENARIO 0: ÁRBOL INICIAL");
            _vista.MostrarDiagrama(_arbol.DiagramaAscii());
            MostrarRecorridos();
            MostrarMetricas();

            _vista.Titulo("ESCENARIO 1: BÚSQUEDAS RÁPIDAS");
            foreach (string nombre in new[] { "Videos", "Anexos", "Zeta" })
            {
                Documento d = _arbol.BuscarConCamino(nombre, out List<string> camino);
                _vista.Mensaje($"Buscar '{nombre}': {(d != null ? "ENCONTRADO" : "NO existe")}");
                _vista.MostrarCamino(camino);
            }

            _vista.Titulo("ESCENARIO 2: ACTUALIZAR (hoja, un hijo y raíz)");
            foreach (string nombre in new[] { "Contratos.pdf", "Tareas", "Proyectos" })
            {
                _arbol.Actualizar(nombre, 999);
                _vista.Mensaje($"Actualizado '{nombre}' (estructura intacta, solo cambia el dato):");
                _vista.MostrarDocumento(_arbol.Buscar(nombre));
            }

            _vista.Titulo("ESCENARIO 3: ELIMINAR HOJA  -> 'Anexos'");
            EliminarYMostrar("Anexos");

            _vista.Titulo("ESCENARIO 4: ELIMINAR NODO CON UN HIJO  -> 'Tareas'");
            EliminarYMostrar("Tareas");

            _vista.Titulo("ESCENARIO 5: ELIMINAR LA RAÍZ  -> 'Proyectos'");
            EliminarYMostrar("Proyectos");

            _vista.Titulo("ESCENARIO 6: ESTADO FINAL");
            MostrarRecorridos();
            MostrarMetricas();
        }

        private void EliminarYMostrar(string nombre)
        {
            _arbol.Eliminar(nombre, out string caso);
            _vista.Mensaje($"Caso aplicado: {caso}");
            _vista.MostrarDiagrama(_arbol.DiagramaAscii());
        }
    }
}

// ---------------------------- PROGRAMA (Main) ----------------------------
namespace TallerMVC
{
    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            new DocumentoController().Iniciar();
        }
    }
}
