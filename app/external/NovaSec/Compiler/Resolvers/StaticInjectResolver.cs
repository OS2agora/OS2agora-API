using System.Collections.Generic;

namespace NovaSec.Compiler.Resolvers
{
    public class StaticInjectResolver : IInjectResolver
    {
        private Dictionary<string, object> _injects;

        public StaticInjectResolver(Dictionary<string, object> injects)
        {
            _injects = injects;
        }

        public bool Exists(string id)
        {
            return _injects.ContainsKey(id);
        }

        public object Resolve(string id)
        {
            return _injects[id];
        }
    }
}