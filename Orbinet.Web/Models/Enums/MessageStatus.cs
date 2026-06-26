namespace Orbinet.Web.Models.Enums
{
    public enum MessageStatus
    {
        EnEspera, //Todavia esta en buffer. 
        EnTransito, // Ya salió y va en ruta.
        Entregado, // llegó al destino
        Fallido, // no llegó al destino, se perdió en la ruta
        Reenviado // fue reenviado a otro nodo por el motor de simulación
    }
}