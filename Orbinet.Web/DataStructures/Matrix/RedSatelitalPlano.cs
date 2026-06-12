using System;
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
}