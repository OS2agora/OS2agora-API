using System;
using System.Collections.Generic;
using System.Text;

namespace Agora.Operations.Common.Exceptions
{
	public class EmptyConclusionException : Exception
	{
		public EmptyConclusionException() : base("Conclusion can not be empty.") { }

		public EmptyConclusionException(string message) : base(message) { }
	}
}
