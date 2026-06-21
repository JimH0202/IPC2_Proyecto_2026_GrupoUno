namespace Orbinet.Web.DataStructures.Antenas;

using Orbinet.Web.Models.Entities;

// Nodo para la lista de antenas terrestres
public class AntenaNode
{
    public GroundAntenna Antenna { get; set; }
    public AntenaNode? Next { get; set; }

    public AntenaNode(GroundAntenna antenna)
    {
        Antenna = antenna;
        Next = null;
    }
}