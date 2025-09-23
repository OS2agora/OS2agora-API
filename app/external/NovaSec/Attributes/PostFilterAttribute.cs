using System;
using System.Collections.Generic;
using System.Linq;

namespace NovaSec.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class PostFilterAttribute : BaseAttribute
    {
        public sealed override string SecurityExpression { get; }
        public List<string> RedactionPaths { get; } = null;

        public PostFilterAttribute(string securityExpression)
        {
            SecurityExpression = securityExpression;
            Key = Sha256Hash(SecurityExpression + (RedactionPaths != null ? string.Join("", RedactionPaths) : ""));
        }

        public PostFilterAttribute(string securityExpression, string redactionPaths) : this(securityExpression)
        {
            if (redactionPaths != null)
            {
                RedactionPaths = redactionPaths.Split(',').Select(s => s.Trim()).ToList();
            }
        }
        
        internal override string Key { get; }
    }
}