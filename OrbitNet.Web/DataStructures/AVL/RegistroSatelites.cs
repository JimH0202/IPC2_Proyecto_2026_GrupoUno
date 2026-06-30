namespace OrbitNet.Web.DataStructures.AVL;

using OrbitNet.Web.DataStructures.Interfaces;
using OrbitNet.Web.Models.Entities;

public class RegistroSatelites : IAbstractCollection
{
    private AvlNode? root;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public void Clear()
    {
        root = null;
        count = 0;
    }

    public void Insert(Satellite satellite)
    {
        if (Search(satellite.Id) != null)
        {
            return;
        }

        root = InsertRecursive(root, satellite);
        count++;
    }

    private AvlNode InsertRecursive(AvlNode? node, Satellite satellite)
    {
        if (node == null)
        {
            return new AvlNode(satellite);
        }

        int compare = string.Compare(satellite.Id, node.Satellite.Id, StringComparison.Ordinal);

        if (compare < 0)
        {
            node.Left = InsertRecursive(node.Left, satellite);
        }
        else if (compare > 0)
        {
            node.Right = InsertRecursive(node.Right, satellite);
        }
        else
        {
            return node;
        }

        UpdateHeight(node);
        return BalanceNode(node);
    }

    public Satellite? Search(string id)
    {
        AvlNode? current = root;

        while (current != null)
        {
            int compare = string.Compare(id, current.Satellite.Id, StringComparison.Ordinal);

            if (compare == 0)
            {
                return current.Satellite;
            }

            current = compare < 0 ? current.Left : current.Right;
        }

        return null;
    }

    public bool Delete(string id)
    {
        if (Search(id) == null)
        {
            return false;
        }

        root = DeleteRecursive(root, id);
        count--;
        return true;
    }

    private AvlNode? DeleteRecursive(AvlNode? node, string id)
    {
        if (node == null)
        {
            return null;
        }

        int compare = string.Compare(id, node.Satellite.Id, StringComparison.Ordinal);

        if (compare < 0)
        {
            node.Left = DeleteRecursive(node.Left, id);
        }
        else if (compare > 0)
        {
            node.Right = DeleteRecursive(node.Right, id);
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right;
            }

            if (node.Right == null)
            {
                return node.Left;
            }

            AvlNode successor = GetMinNode(node.Right);
            node.Satellite = successor.Satellite;
            node.Right = DeleteRecursive(node.Right, successor.Satellite.Id);
        }

        UpdateHeight(node);
        return BalanceNode(node);
    }

    private AvlNode GetMinNode(AvlNode node)
    {
        AvlNode current = node;

        while (current.Left != null)
        {
            current = current.Left;
        }

        return current;
    }

    public string TraverseInOrder()
    {
        return TraverseInOrderRecursive(root);
    }

    private string TraverseInOrderRecursive(AvlNode? node)
    {
        if (node == null)
        {
            return "";
        }

        string result = "";

        result += TraverseInOrderRecursive(node.Left);
        result += $"{node.Satellite.Id} | {node.Satellite.Name} | {node.Satellite.Ip}\n";
        result += TraverseInOrderRecursive(node.Right);

        return result;
    }

    public AvlNode? GetRoot()
    {
        return root;
    }

    private AvlNode BalanceNode(AvlNode node)
    {
        int balance = GetBalance(node);

        if (balance > 1)
        {
            if (GetBalance(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left!);
            }

            return RotateRight(node);
        }

        if (balance < -1)
        {
            if (GetBalance(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right!);
            }

            return RotateLeft(node);
        }

        return node;
    }

    private AvlNode RotateRight(AvlNode y)
    {
        AvlNode x = y.Left!;
        AvlNode? temp = x.Right;

        x.Right = y;
        y.Left = temp;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private AvlNode RotateLeft(AvlNode x)
    {
        AvlNode y = x.Right!;
        AvlNode? temp = y.Left;

        y.Left = x;
        x.Right = temp;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }

    private void UpdateHeight(AvlNode node)
    {
        int leftHeight = GetHeight(node.Left);
        int rightHeight = GetHeight(node.Right);

        node.Height = 1 + GetMax(leftHeight, rightHeight);
    }

    private int GetHeight(AvlNode? node)
    {
        return node == null ? 0 : node.Height;
    }

    private int GetBalance(AvlNode? node)
    {
        return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
    }

    private int GetMax(int a, int b)
    {
        return a > b ? a : b;
    }
}
