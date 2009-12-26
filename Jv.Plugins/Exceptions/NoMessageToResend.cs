using System;

namespace Jv.Plugins.Exceptions
{
	public class NoMessageToResend : Exception
	{
		#region Constructors
		public NoMessageToResend() {}
		public NoMessageToResend(string message) : base(message) {}
		#endregion
	}
}