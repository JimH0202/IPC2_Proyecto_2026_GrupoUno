namespace OrbitNet.Web.DataStructures.Antenas;

using OrbitNet.Web.DataStructures.Listas;
using OrbitNet.Web.Models.Entities;

public class ListaAntenas
{
    private ListaEnlazada<GroundAntenna> lista = new();

    public int Count => lista.Count;
    public bool IsEmpty => lista.IsEmpty;

    public void Add(GroundAntenna antena)
    {
        lista.Add(antena);
    }

    public GroundAntenna? GetAt(int index)
    {
        return lista.Get(index);
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
