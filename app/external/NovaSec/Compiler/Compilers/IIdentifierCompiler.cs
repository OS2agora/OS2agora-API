using System.Linq.Expressions;
using NovaSec.Compiler.Resolvers;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler.Compilers
{
    public interface IIdentifierCompiler<TInstanceResolver> where TInstanceResolver : IIdentifierResolver
    {
        Expression Compile(Identifier i, ParameterExpression identifierInstanceResolver);
    }
}