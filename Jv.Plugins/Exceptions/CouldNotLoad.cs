using System;

namespace Jv.Plugins.Exceptions
{
	public class CouldNotLoad : Exception
	{
		#region Fields
		public string DllPath { get; private set; }
		public Type PluginType { get; private set; }
		#endregion

		#region Constructors
		public CouldNotLoad(string dllPath, Type pluginType, string message, Exception innerException)
			: base(message, innerException)
		{
			DllPath = dllPath;
			PluginType = pluginType;
		}

		public CouldNotLoad(string dllPath, Type pluginType)
			: this(dllPath, pluginType, string.Format("Plugin \"{0}\" could not be loaded.", dllPath), null) { }

		public CouldNotLoad(string dllPath, Type pluginType, string message)
			: this(dllPath, pluginType, message, null) { }

		public CouldNotLoad(string dllPath, Type pluginType, Exception innerException)
			: this(dllPath, pluginType, string.Format("Plugin \"{0}\" could not be loaded.", dllPath), innerException) { }
		#endregion
	}
}