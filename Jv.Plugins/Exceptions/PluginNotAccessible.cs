using System;

namespace Jv.Plugins.Exceptions
{
	public class PluginNotAccessible : Exception
	{
		#region Fields
		public Type PluginType { get; private set; }
		#endregion

		#region Constructors
		public PluginNotAccessible(Type pType)
			: base(string.Format("Plugin of type \"{0}\" could not be accessed.", pType))
		{
			PluginType = pType;
		}
		#endregion
	}
}