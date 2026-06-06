namespace Orbinet.Web.DataStructures.Logs;

public class LogNode
{
    public DateTime FechaHora { get; set; } //fecha y hora del evento
    public string Gravedad { get; set; } //Importacia de evento 
    public string Mensaje { get; set; } //Descripcion del evento
    public LogNode? Siguiente { get; set; } //Referencia al siguiente nodo en la lista
    public LogNode? Anterior { get; set; }   //Referencia al nodo anterior en la lista
}