namespace OrbitNet.Web.DataStructures.Antenas;

using OrbitNet.Web.DataStructures.Listas;
using OrbitNet.Web.Models.Entities;

/// Punto de integracion para Graphviz:
// La lista de antenas puede recorrerse desde la estructura ListaEnlazada
// para generar la representacion grafica.

public class ListaAntenas
{
    private ListaEnlazada<GroundAntenna> lista = new();

    public int Count => lista.Count;
    public bool IsEmpty => lista.IsEmpty;

    public void Add(GroundAntenna antena)
    {
        lista.Add(antena);
    }

    // El metodo busca una antena en la lista por su identificador unico (id).
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

    // El metodo busca una antena en la lista por su direccion IP.
    public GroundAntenna? SearchByIp(string ip)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            var antena = lista.Get(i);

            if (antena != null && antena.Ip == ip)
            {
                return antena;
            }
        }

        return null;
    }

    public void Clear()
    {
        lista.Clear();
    }
}
