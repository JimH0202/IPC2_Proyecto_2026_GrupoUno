namespace Orbinet.Web.DataStructures.Matrix;

public class MatrixNode
{
    public int Row { get; set; }
    public int Column { get; set; }

    public string SatelliteId { get; set; }
    public string IpAddress { get; set; }

    public MatrixNode? Up { get; set; }
    public MatrixNode? Down { get; set; }
    public MatrixNode? Left { get; set; }
    public MatrixNode? Right { get; set; }

    public MatrixNode(int row,int column,string satelliteId,string ipAddress)
    {
        Row = row;
        Column = column;
        SatelliteId = satelliteId;
        IpAddress = ipAddress;

        Up = null;
        Down = null;
        Left = null;
        Right = null;
    }
}