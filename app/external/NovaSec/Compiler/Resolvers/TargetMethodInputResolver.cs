using System;
using System.Linq;
using System.Reflection;
using NovaSec.Exceptions;

namespace NovaSec.Compiler.Resolvers
{
    public class TargetMethodInputResolver : IIdentifierResolver
    {
        private readonly MethodInfo _targetMethodInfo;
        private readonly object[] _targetMethodArguments;
        
        public TargetMethodInputResolver() { }

        public TargetMethodInputResolver(MethodInfo targetMethodInfo, object[] targetMethodArguments)
        {
            _targetMethodArguments = targetMethodArguments;
            _targetMethodInfo = targetMethodInfo;
        }

        public object GetValueByName(string argumentName)
        {
            var targetMethodParams = _targetMethodInfo.GetParameters();
            var paramIdx = Array.FindIndex(targetMethodParams, p => p.Name == argumentName);
            if (paramIdx < 0)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} cannot resolve input parameter for {argumentName} because the parameter was not found on the target method! Available parameters were {string.Join(", ", targetMethodParams.Select(p => p.Name))}");
            }

            if (_targetMethodArguments.Length < paramIdx)
            {
                throw new SecurityExpressionResolveException(
                    $"{this.GetType()} cannot resolve input parameter for {argumentName} because the parameter was not provided to the resolver");
            }

            return _targetMethodArguments[paramIdx];
        }
    }
}