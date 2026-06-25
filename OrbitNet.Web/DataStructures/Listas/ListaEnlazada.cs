using OrbitNet.Web.DataStructures.Interfaces;

namespace OrbitNet.Web.DataStructures.Listas;

public class ListaEnlazada<T> : IAbstractColletion
{
    private NodoLista<T>? cabeza;
    private NodoLista<T>? cola;
    private int cantidad;

    public int Count => cantidad;
    public bool IsEmpty => cantidad == 0;

    public void Clear()
    {
        cabeza = null;
        cola = null;
        cantidad = 0;
    }

    public void Add(T item)
    {
        NodoLista<T> nuevo = new NodoLista<T>(item);

        if (IsEmpty)
        {
            cabeza = nuevo;
            cola = nuevo;
        }
        else
        {
            cola!.Siguiente = nuevo;
            cola = nuevo;
        }
        cantidad++;
    }

    public T? Get(int index)
    {
        if (index < 0 || index >= cantidad)
            return default;

        NodoLista<T>? actual = cabeza;
        for (int i = 0; i < index; i++)
        {
            actual = actual!.Siguiente;
        }
        return actual!.Valor;
    }

    public bool Remove(T item)
    {
        NodoLista<T>? actual = cabeza;
        NodoLista<T>? anterior = null;

        while (actual != null)
        {
            if (actual.Valor!.Equals(item))
            {
                if (anterior == null)
                    cabeza = actual.Siguiente;
                else
                    anterior.Siguiente = actual.Siguiente;

                if (actual == cola)
                    cola = anterior;

                cantidad--;
                return true;
            }
            anterior = actual;
            actual = actual.Siguiente;
        }
        return false;
    }
}
