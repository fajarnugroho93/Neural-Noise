namespace SpaceKomodo.Utilities
{
    public interface IInitializable<in T1>
    {
        void Initialize(T1 t1);
    }
}