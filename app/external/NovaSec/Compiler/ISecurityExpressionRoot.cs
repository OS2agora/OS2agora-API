using System.Collections.Generic;

namespace NovaSec.Compiler
{
    public interface ISecurityExpressionRoot
    {
        bool HasRole(string role);
        bool HasAllRoles(List<string> roles);
        bool HasAnyRole(List<string> roles);
    }
}