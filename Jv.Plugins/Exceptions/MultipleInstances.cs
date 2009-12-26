using System;

namespace Jv.Plugins.Exceptions
{
	public class MultipleInstances : Exception
	{
		#region Fields
		public Type PluginType { get; private set; }
		#endregion

		#region Constructors
		public MultipleInstances(Type pluginType)
		{
			PluginType = pluginType;
		}
		#endregion
	}
}