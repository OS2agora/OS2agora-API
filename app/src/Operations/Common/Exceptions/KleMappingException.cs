using Agora.Models.Models;
using System;
using System.Collections.Generic;

namespace Agora.Operations.Common.Exceptions
{
    public class KleMappingException : Exception
    {
        public List<KleHierarchy> KleHierarchies { get; }

        public KleMappingException() : base("An error occurred when validating kleMappings.")
        {
        }

        public KleMappingException(string message) : base(message)
        {
        }

        public KleMappingException(List<KleHierarchy> kleHierarchies) : this()
        {
            KleHierarchies = kleHierarchies;
        }
    }
}