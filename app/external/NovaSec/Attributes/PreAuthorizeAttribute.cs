using System;

namespace NovaSec.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class PreAuthorizeAttribute : BaseAttribute
    {
        public sealed override string SecurityExpression { get; }

        public PreAuthorizeAttribute(string securityExpression)
        {
            SecurityExpression = securityExpression;
            Key = Sha256Hash(SecurityExpression);
        }
        
        internal override string Key { get; }
    }
}