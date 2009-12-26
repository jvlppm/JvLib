using System;

namespace Jv.Plugins.Exceptions
{
	public class CouldNotInstantiate : CouldNotLoad
	{
		#region Constructors
		public CouldNotInstantiate(string dllPath, Type pluginType, Exception innerException)
			: base(dllPath, pluginType, innerException) {}
		#endregion
	}
}