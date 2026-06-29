namespace OrbitNet.Web.DataStructures.Matrix;

public class HeaderNode
{
    public int Index { get; set; }

    public HeaderNode? Previous { get; set; } // Referencia al nodo anterior en la lista de encabezados
    public HeaderNode? Next { get; set; } // Referencia al nodo siguiente en la lista de encabezados

    public MatrixNode? Access { get; set; } // Referencia al primer nodo de la fila o columna correspondiente a este encabezado

    public HeaderNode(int index)
    {
        Index = index;
        Previous = null;
        Next = null;
        Access = null;
    }
}