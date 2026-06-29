namespace OrbitNet.Web.DataStructures.Antenas;

using OrbitNet.Web.Models.Entities;

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
