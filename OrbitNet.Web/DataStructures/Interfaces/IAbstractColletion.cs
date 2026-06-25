namespace OrbitNet.Web.DataStructures.Interfaces;
public interface IAbstractColletion
{
    int Count { get; }
    void Clear();
    bool IsEmpty{get;}
}