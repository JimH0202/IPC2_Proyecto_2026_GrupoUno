namespace OrbitNet.Web.DataStructures.Listas;

public class NodoLista<T>
{
    public T Valor { get; set; }
    public NodoLista<T>? Siguiente { get; set; }

    public NodoLista(T valor)
    {
        Valor = valor;
        Siguiente = null;
    }
}
