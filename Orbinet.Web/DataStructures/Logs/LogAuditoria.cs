namespace Orbinet.Web.DataStructures.Logs;
using System;
using System.Text.RegularExpressions;

public class LogAuditoria : IAbstractCollection
{
    private NodoLog cabeza;
    private NodoLog cola;
    private int cantidad;

    public int Count => cantidad;
    public bool IsEmpty => cantidad == 0;

    public void Clear()
    {
        cabeza = null;
        cola = null;
        cantidad = 0;
    }

    public void EscribirEvento(string gravedad, string mensaje)
    {
        NodoLog nuevo = new NodoLog();
        nuevo.FechaHora = DateTime.Now;
        nuevo.Gravedad = gravedad;
        nuevo.Mensaje = mensaje;

        if (IsEmpty)
        {
            cabeza = nuevo;
            cola = nuevo;
        }
        else
        {
            cola.Siguiente = nuevo;
            nuevo.Anterior = cola;
            cola = nuevo;
        }
        cantidad++;
    }

    public void BuscarPorRegex(string patron)
    {
        Regex regex = new Regex(patron);
        NodoLog actual = cabeza;

        while (actual != null)
        {
            if (regex.IsMatch(actual.Mensaje))
            {
                Console.WriteLine(
                    $"Coincidencia encontrada: {actual.Gravedad} - {actual.Mensaje} ({actual.FechaHora})"
                );
            }
            actual = actual.Siguiente;
        }
    }
}
