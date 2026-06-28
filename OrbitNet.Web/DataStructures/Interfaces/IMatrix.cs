namespace OrbitNet.Web.DataStructures.Interfaces;

public interface IMatrix
{
    int Count { get; }
    bool IsEmpty { get; }
    void Clear();
}