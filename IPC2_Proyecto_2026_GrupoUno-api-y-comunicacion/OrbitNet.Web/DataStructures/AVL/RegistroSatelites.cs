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

    // El metodo realiza la insercion de un satelite en el arbol AVL, asegurando que no se inserten duplicados y manteniendo el balance del arbol.
    public void Insert(Satellite satellite)
    {
        if (Search(satellite.Id) != null)
        {
            return;
        }

        root = InsertRecursive(root, satellite);
        count++;
    }

    // El metodo realiza la insercion recursiva de un satelite en el arbol AVL, comparando los identificadores de los 
    // satelites para determinar la posicion correcta y asegurando que el arbol se mantenga balanceado
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


    // El metodo busca un satelite en el arbol AVL comparando los identificadores de los satelites y devolviendo el 
    // satelite encontrado o null si no se encuentra
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

    // El metodo elimina un satelite del arbol AVL comparando los identificadores de los satelites y devolviendo true si se 
    // elimino correctamente o false si no se encontro el satelite
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

    // El metodo realiza la eliminacion recursiva de un satelite en el arbol AVL, comparando los identificadores de los satelites para 
    // determinar la posicion correcta
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

    // El metodo obtiene el nodo con el valor minimo en un subarbol, utilizado para encontrar el sucesor en la eliminacion de un nodo
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

    // El metodo realiza un recorrido en orden del arbol AVL, devolviendo una cadena con los identificadores 
    // nombres e IPs de los satelites
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

    // El metodo devuelve la raiz del arbol AVL, utilizado para pruebas y verificaciones
    // Punto de integracion para Graphviz:
    // Utilizar este nodo como inicio para recorrer el arbol AVL
    // y generar la representacion grafica.
    public AvlNode? GetRoot()
    {
        return root;
    }

    // El metodo balancea un nodo del arbol AVL, realizando rotaciones si es necesario para mantener el balance del arbol
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

// El metodo realiza una rotacion a la derecha en un nodo del arbol AVL, utilizada para balancear el arbol
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

    // El metodo realiza una rotacion a la izquierda en un nodo del arbol AVL, utilizada para balancear el arbol
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

    // El metodo actualiza la altura de un nodo del arbol AVL, utilizada para mantener el balance del arbol
    private void UpdateHeight(AvlNode node)
    {
        int leftHeight = GetHeight(node.Left);
        int rightHeight = GetHeight(node.Right);

        node.Height = 1 + GetMax(leftHeight, rightHeight);
    }

    // El metodo obtiene la altura de un nodo del arbol AVL, utilizada para calcular el balance del arbol
    private int GetHeight(AvlNode? node)
    {
        return node == null ? 0 : node.Height;
    }

    // El metodo obtiene el balance de un nodo del arbol AVL, utilizada para determinar si se necesita realizar una rotacion
    private int GetBalance(AvlNode? node)
    {
        return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
    }

    // El metodo obtiene el valor maximo entre dos enteros, utilizada para calcular la altura de un nodo
    private int GetMax(int a, int b)
    {
        return a > b ? a : b;
    }
}