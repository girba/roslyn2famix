namespace Famix.Language
{
    using Famix.Language.Contracts;

    public interface IFamixContainer<in T> : IFamixNode
    {
        void Add(T node);
    }
}
