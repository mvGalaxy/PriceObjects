namespace PriceObjects
{
    public interface INewInstanceReplicator<T>
    {
        T CreateNewInstance();
    }
}