namespace Jv.Plugins.Exceptions
{
	public class NotAllowed : RemoteException
	{
		#region Constructors
		public NotAllowed() : base("Operation Not Allowed") {}
		public NotAllowed(string message) : base(message) {}
		#endregion
	}
}