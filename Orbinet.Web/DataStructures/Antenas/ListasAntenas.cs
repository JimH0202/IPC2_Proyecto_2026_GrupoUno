namespace Orbinet.Web.DataStructures.Antenas;

using Orbinet.Web.DataStructures.Interfaces;
using Orbinet.Web.Models.Entities;

public class ListaAntenas : IAbstractCollection
{
    private AntenaNode? head;
    private AntenaNode? tail;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    // El metodo inserta una nueva antena terrestre al final de la lista enlazada, actualizando el puntero tail y el contador de elementos.
    public void Add(GroundAntenna antenna)
    {
        AntenaNode nuevo = new AntenaNode(antenna);

        if (head == null)
        {
            head = nuevo;
            tail = nuevo;
        }
        else
        {
            tail!.Next = nuevo;
            tail = nuevo;
        }

        count++;
    }

    // El metodo recorre la lista enlazada de antenas terrestres buscando una antena con el id especificado.
    public GroundAntenna? SearchById(string id)
    {
        AntenaNode? current = head;

        while (current != null)
        {
            if (current.Antenna.Id == id)
            {
                return current.Antenna;
            }

            current = current.Next;
        }

        return null;
    }

    // El metodo elimina todas las antenas terrestres de la lista enlazada, estableciendo head y tail a null y reiniciando el contador de elementos a cero.
    public void Clear()
    {
        head = null;
        tail = null;
        count = 0;
    }
}