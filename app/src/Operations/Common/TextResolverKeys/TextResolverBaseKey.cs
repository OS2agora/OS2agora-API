namespace Agora.Operations.Common.TextResolverKeys
{
    public abstract class TextResolverBaseKey
    {
        public string Value { get; }

        protected TextResolverBaseKey(string value)
        {
            Value = value;
        }
    }
}