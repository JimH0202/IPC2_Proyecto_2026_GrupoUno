using System;
using System.Runtime.CompilerServices;
using Orbinet.Web.DataStructures.Interfaces;
namespace Orbinet.Web.DataStructures.Matrix;
public class RedSatelitalPlano : IMatrix
{
    private HeaderNode? rowHeaders;
    private HeaderNode? columnHeaders;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public void Clear()
    {
        rowHeaders = null;
        columnHeaders = null;
        count = 0;
    }

    public MatrixNode? Search(int row, int column)
    {
        HeaderNode? rowHeader = SearchHeader(rowHeaders, row);

        if(rowHeader == null)
        {
            return null;
        }

        MatrixNode? current = rowHeader.Access;

        while(current != null)
        {
            if(current.Column == column)
            {
                return current;
            }

            if(current.Column > column)
            {
                return null;
            }

            current = current.Right;
        }
        return null;
    }

// El metodo devuelve un booleano indicando si la inserción fue exitosa o no
    public bool Insert(int row, int column, string satelliteId, string ipAddress)
    {
        if(Search(row, column) != null)
        {
            return false;
        }

        MatrixNode newNode = new MatrixNode(row, column, satelliteId, ipAddress);
        HeaderNode rowHeader = GetOrCreateHeader(ref rowHeaders, row);
        HeaderNode columnHeader = GetOrCreateHeader(ref columnHeaders, column);

        InsertInRow(rowHeader, newNode);
        InsertInColumn(columnHeader,newNode);

        count++;
        return true;
    }

// El metodo devuelve un booleano indicando si la eliminación fue exitosa o no
    public bool Delete(int row, int column)
    {
        MatrixNode? node = Search(row, column);

        if (node == null)
        {
            return false;
        }

        // Desconectar horizontalmente
        if (node.Left != null)
        {
            node.Left.Right = node.Right;
        }
        else
        {
            HeaderNode? rowHeader = SearchHeader(rowHeaders, row);
            if (rowHeader != null)
            {
                rowHeader.Access = node.Right;
            }
        }

        if (node.Right != null)
        {
            node.Right.Left = node.Left;
        }

        // Desconectar verticalmente
        if (node.Up != null)
        {
            node.Up.Down = node.Down;
        }
        else
        {
            HeaderNode? columnHeader = SearchHeader(columnHeaders, column);
            if (columnHeader != null)
            {
                columnHeader.Access = node.Down;
            }
        }

        if (node.Down != null)
        {
            node.Down.Up = node.Up;
        }

        node.Left = null;
        node.Right = null;
        node.Up = null;
        node.Down = null;

        count--;
        return true;
    }

// Metodos privados para manejar los encabezados y la insercion de nodos en filas y columnas
    private HeaderNode? SearchHeader(HeaderNode? start, int index)
    {
        HeaderNode? current = start;

        while (current != null)
        {
            if (current.Index == index)
            {
                return current;
            }

            if (current.Index > index)
            {
                return null;
            }

            current = current.Next;
        }

        return null;
    }

// El metodo GetOrCreateHeader devuelve el nodo de encabezado correspondiente al indice dado lo crea si no existe
    private HeaderNode GetOrCreateHeader(ref HeaderNode? start, int index)
    {
        if (start == null)
        {
            start = new HeaderNode(index);
            return start;
        }

        HeaderNode? current = start;
        HeaderNode? previous = null;

        while (current != null && current.Index < index)
        {
            previous = current;
            current = current.Next;
        }

        if (current != null && current.Index == index)
        {
            return current;
        }

        HeaderNode newHeader = new HeaderNode(index);

        if (previous == null)
        {
            newHeader.Next = start;
            start.Previous = newHeader;
            start = newHeader;
        }
        else
        {
            newHeader.Next = current;
            newHeader.Previous = previous;
            previous.Next = newHeader;

            if (current != null)
            {
                current.Previous = newHeader;
            }
        }

        return newHeader;
    }

// El metodo inserta un nodo en la fila correspondiente al encabezado dado manteniendo el orden de las columnas
    private void InsertInRow(HeaderNode rowHeader, MatrixNode newNode)
    {
        if (rowHeader.Access == null)
        {
            rowHeader.Access = newNode;
            return;
        }

        MatrixNode? current = rowHeader.Access;
        MatrixNode? previous = null;

        while (current != null && current.Column < newNode.Column)
        {
            previous = current;
            current = current.Right;
        }

        if (previous == null)
        {
            newNode.Right = rowHeader.Access;
            rowHeader.Access.Left = newNode;
            rowHeader.Access = newNode;
        }
        else
        {
            newNode.Right = current;
            newNode.Left = previous;
            previous.Right = newNode;

            if (current != null)
            {
                current.Left = newNode;
            }
        }
    }
// El metodo inserta un nodo en la columna correspondiente al encabezado dado manteniendo el orden de las filas
    private void InsertInColumn(HeaderNode columnHeader, MatrixNode newNode)
    {
        if (columnHeader.Access == null)
        {
            columnHeader.Access = newNode;
            return;
        }

        MatrixNode? current = columnHeader.Access;
        MatrixNode? previous = null;

        while (current != null && current.Row < newNode.Row)
        {
            previous = current;
            current = current.Down;
        }

        if (previous == null)
        {
            newNode.Down = columnHeader.Access;
            columnHeader.Access.Up = newNode;
            columnHeader.Access = newNode;
        }
        else
        {
            newNode.Down = current;
            newNode.Up = previous;
            previous.Down = newNode;

            if (current != null)
            {
                current.Up = newNode;
            }
        }
    }
}
