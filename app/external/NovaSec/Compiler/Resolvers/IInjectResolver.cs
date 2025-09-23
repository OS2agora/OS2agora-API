namespace NovaSec.Compiler.Resolvers
{
    public interface IInjectResolver
    {
        bool Exists(string id);
        object Resolve(string id);
    }
}