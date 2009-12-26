using System;

namespace Jv.Plugins.Exceptions
{
	public class RemoteException : Exception
	{
		#region Constructors
		public RemoteException(string message) : base(message) {}
		public RemoteException(string message, Exception ex) : base(message, ex) {}
		#endregion
	}
}