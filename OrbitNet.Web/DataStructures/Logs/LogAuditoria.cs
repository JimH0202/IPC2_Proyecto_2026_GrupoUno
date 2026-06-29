namespace OrbitNet.Web.DataStructures.Logs;

using System.Text.RegularExpressions;
using OrbitNet.Web.DataStructures.Interfaces;

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

    public LogNode? GetHead()
    {
        return cabeza;
    }

    public void ForEach(Action<LogNode> action)
    {
        var actual = cabeza;
        while (actual != null)
        {
            action(actual);
            actual = actual.Next;
        }
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
