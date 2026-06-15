namespace Orbinet.Web.DataStructures.Logs;

public class LogNode
{
    public DateTime Timestamp { get; set; } //fecha y hora del evento
    public string Severity { get; set; } //Importacia de evento 
    public string Message { get; set; } //Descripcion del evento
    public LogNode? Next { get; set; } //Referencia al siguiente nodo en la lista
    public LogNode(String severity, string message)
    {
        Timestamp = DateTime.Now;
        Severity = severity;
        Message = message;
        Next = null;
    }
}