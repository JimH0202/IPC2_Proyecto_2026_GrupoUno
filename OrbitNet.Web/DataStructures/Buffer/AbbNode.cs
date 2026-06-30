namespace OrbitNet.Web.DataStructures.Buffer;

using OrbitNet.Web.Models.Entities;

public class AbbNode
{
    public MessagePacket Packet { get; set; }

    public AbbNode? Left { get; set; }
    public AbbNode? Right { get; set; }

    public AbbNode(MessagePacket packet)
    {
        Packet = packet;
        Left = null;
        Right = null;
    }
}
