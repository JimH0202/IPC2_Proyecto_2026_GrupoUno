namespace OrbitNet.Web.DataStructures.Antenas;

using OrbitNet.Web.DataStructures.Listas;
using OrbitNet.Web.Models.Entities;

// Punto de integracion para Graphviz
// La lista enlazada de antenas puede recorrerse iniciando desde 'head'
// para generar la representacion grafica de la estructura.

public class ListaAntenas
{
    private ListaEnlazada<GroundAntenna> lista = new();

    public int Count => lista.Count;
    public bool IsEmpty => lista.IsEmpty;

    public void Add(GroundAntenna antena)
    {
        lista.Add(antena);
    }

    public GroundAntenna? SearchById(string id)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            var antena = lista.Get(i);
            if (antena != null && antena.Id == id)
                return antena;
        }
        return null;
    }

    public void Clear()
    {
        lista.Clear();
    }
}
