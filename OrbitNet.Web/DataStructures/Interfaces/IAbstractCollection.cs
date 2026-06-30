
namespace OrbitNet.Web.DataStructures.Interfaces;
public interface IAbstractCollection
{
    int Count { get; }
    bool IsEmpty { get; }
    void Clear();
}
