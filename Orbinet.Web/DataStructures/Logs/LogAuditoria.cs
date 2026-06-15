namespace Orbinet.Web.DataStructures.Logs;
using System;
using System.Text.RegularExpressions;
using Orbinet.Web.DataStructures.Interfaces;

public class LogAuditoria
{
    private LogNode? cabeza;
    private LogNode? cola;
    private int cantidad;

    public int Count => cantidad;
    public bool IsEmpty => cantidad == 0;

    public void Clear()
    {
        cabeza = null;
        cola = null;
        cantidad = 0;
    }

    public void WriteEvent(string severity, string message)
    {
        LogNode nuevo = new LogNode(severity, message);

        if (IsEmpty)
        {
            cabeza = nuevo;
            cola = nuevo;
        }
        else
        {
            cola!.Next = nuevo;
            cola = nuevo;
        }
        cantidad++;
    }

    public string SearchLogRegex(string pattern)
    {
        Regex regex = new Regex(pattern);
        LogNode? actual = cabeza;
        string resultado = "";

        while (actual != null)
        {
            if (regex.IsMatch(actual.Message))
            {
                resultado += $"{actual.Timestamp} | {actual.Severity} | {actual.Message}";
            }
            actual = actual.Next;
        }
        return resultado;
    }
}
