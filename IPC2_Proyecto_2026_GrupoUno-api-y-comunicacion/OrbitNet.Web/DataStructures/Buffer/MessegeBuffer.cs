namespace OrbitNet.Web.DataStructures.Buffer;

using OrbitNet.Web.DataStructures.Interfaces;
using OrbitNet.Web.Models.Entities;

public class BufferMensajes : IMessageBuffer
{
    private AbbNode? root;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public void Clear()
    {
        root = null;
        count = 0;
    }

    // El metodo Enqueue inserta un nuevo mensaje en el ABB basado en su prioridad.
    public void Enqueue(MessagePacket packet)
    {
        AbbNode nuevo = new AbbNode(packet);

        if (root == null)
        {
            root = nuevo;
            count++;
            return;
        }

        InsertRecursive(root, nuevo);
        count++;
    }

    // Es un metodo auxiliar que se encarga de insertar el nuevo nodo en la posicion correcta del ABB para mantener el orden basado en la prioridad del mensaje.
    private void InsertRecursive(AbbNode current, AbbNode nuevo)
    {
        if (nuevo.Packet.Priority < current.Packet.Priority)
        {
            if (current.Left == null)
            {
                current.Left = nuevo;
            }
            else
            {
                InsertRecursive(current.Left, nuevo);
            }
        }
        else
        {
            if (current.Right == null)
            {
                current.Right = nuevo;
            }
            else
            {
                InsertRecursive(current.Right, nuevo);
            }
        }
    }

    // El metodo Peek devuelve el mensaje con la mayor prioridad (el nodo mas a la derecha del ABB) sin eliminarlo de la estructura.
    public MessagePacket? Peek()
    {
        if (root == null)
        {
            return null;
        }

        AbbNode current = root;

        while (current.Right != null)
        {
            current = current.Right;
        }

        return current.Packet;
    }

    // El metodo Dequeue elimina y devuelve el mensaje con la mayor prioridad (el nodo mas a la derecha del ABB). Si el ABB esta vacio, devuelve null.
    public MessagePacket? Dequeue()
    {
        if (root == null)
        {
            return null;
        }

        MessagePacket? packet = Peek();

        root = DeleteMax(root);
        count--;

        return packet;
    }

    // Es un metodo auxiliar que se encarga de eliminar el nodo con la mayor prioridad (el nodo mas a la derecha) del ABB y reestructurar el arbol para mantener su propiedad de orden.
    private AbbNode? DeleteMax(AbbNode? current)
    {
        if (current == null)
        {
            return null;
        }

        if (current.Right == null)
        {
            return current.Left;
        }

        current.Right = DeleteMax(current.Right);
        return current;
    }

    // El metodo busca un mensaje en el ABB basado en su codigo hexadecimal y devuelve el mensaje si se encuentra o null si no existe.
    public MessagePacket? SearchByHexCode(string hexCode)
    {
        return SearchByHexCodeRecursive(root, hexCode);
    }

    // Es un metodo auxiliar que se encarga de realizar una busqueda recursiva en el ABB para encontrar un mensaje con el codigo hexadecimal especificado.
    private MessagePacket? SearchByHexCodeRecursive(AbbNode? current, string hexCode)
    {
        if (current == null)
        {
            return null;
        }

        if (current.Packet.CodHex == hexCode)
        {
            return current.Packet;
        }

        MessagePacket? leftResult = SearchByHexCodeRecursive(current.Left, hexCode);

        if (leftResult != null)
        {
            return leftResult;
        }

        return SearchByHexCodeRecursive(current.Right, hexCode);
    }

    // El metodo devuelve una cadena que representa el recorrido inorden del ABB, mostrando los mensajes ordenados por prioridad de menor a mayor.
    public string TraverseInOrder()
    {
        return TraverseInOrderRecursive(root);
    }

    private string TraverseInOrderRecursive(AbbNode? current)
    {
        if (current == null)
        {
            return "";
        }

        string result = "";

        result += TraverseInOrderRecursive(current.Left);
        result += $"{current.Packet.CodHex} | Prioridad: {current.Packet.Priority} | Destino: {current.Packet.DestinationIp}\n";
        result += TraverseInOrderRecursive(current.Right);

        return result;
    }

    // Metodos publicos para la interfaz IMessageBuffer
    public void Agregar(MessagePacket packet)
    {
        Enqueue(packet);
    }

    public MessagePacket? ObtenerSiguiente()
    {
        return Dequeue();
    }

    public MessagePacket? VerSiguiente()
    {
        return Peek();
    }

    public MessagePacket? BuscarPorCodigoHex(string hexCode)
    {
        return SearchByHexCode(hexCode);
    }

    // Punto de integracion para Graphviz:
    // El recorrido del arbol puede utilizarse como base para generar
    // la representacion gráfica del Buffer de Mensajes

    public string RecorrerInOrden()
    {
        return TraverseInOrder();
    }
}
