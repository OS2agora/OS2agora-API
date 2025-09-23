using System;
using System.Security.Cryptography;
using System.Text;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Attributes
{
    public abstract class BaseAttribute : Attribute
    {
        internal IExpression TreeExpression { get; set; }

        protected static string Sha256Hash(string value)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        internal abstract string Key { get; }

        public abstract string SecurityExpression { get; }
    }
}