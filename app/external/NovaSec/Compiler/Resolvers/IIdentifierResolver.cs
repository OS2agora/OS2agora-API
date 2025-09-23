namespace NovaSec.Compiler.Resolvers
{
    /// <summary>
    /// This interface represent the series of objects that are provided to a compiled linq expression when executing
    /// it. The respective compilers and resolvers must be compatible for the generated code to actually function when
    /// executed. i.e. the compiler must make assumptions about, what can be extracted from the resolver when it's
    /// running even though this interface is unable to express exactly how that should happen.
    /// </summary>
    public interface IIdentifierResolver
    {
    }
}