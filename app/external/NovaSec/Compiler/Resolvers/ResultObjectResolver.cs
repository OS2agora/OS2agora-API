using System.Reflection;

namespace NovaSec.Compiler.Resolvers
{
    public class ResultObjectResolver<TResultType> : TargetMethodInputResolver
    {
        public TResultType ResultObject { get; }

        public ResultObjectResolver(TResultType result) : this(result, null, null)
        {
        }

        public ResultObjectResolver(TResultType result, MethodInfo targetMethodInfo, object[] targetMethodArguments) :
            base(targetMethodInfo, targetMethodArguments)
        {
            ResultObject = result;
        }
    }
}