namespace Orbinet.Web.DataStructures.AVL;

using Orbinet.Web.Models.Entities;

public class AvlNode
{
    public Satellite Satellite { get; set; }
    public int Height { get; set; }

    public AvlNode? Left { get; set; }
    public AvlNode? Right { get; set; }

    public AvlNode(Satellite satellite)
    {
        Satellite = satellite;
        Height = 1;
        Left = null;
        Right = null;
    }
}