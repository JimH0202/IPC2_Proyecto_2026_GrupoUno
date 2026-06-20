namespace Orbinet.Web.DataStructures.Buffer;

using Orbinet.Web.DataStructures.Interfaces;
using Orbinet.Web.Models.Entities;

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

    public MessagePacket? SearchByHexCode(string hexCode)
    {
        return SearchByHexCodeRecursive(root, hexCode);
    }

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
}