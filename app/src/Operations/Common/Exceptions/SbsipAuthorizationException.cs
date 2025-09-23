using System;
using System.Collections.Generic;
using System.Text;

namespace BallerupKommune.Operations.Common.Exceptions
{
	public class SbsipAuthorizationException : Exception
	{
		public SbsipAuthorizationException() : base("Sbsip authorization failed") { }

		public SbsipAuthorizationException(string message) : base(message) { }
	}
}
