using System;

namespace Jv.Plugins.Exceptions
{
	public class ClassMultipleDefinition : CouldNotLoad
	{
		#region Constructors
		public ClassMultipleDefinition(string dllPath, Type pluginType)
			: base(dllPath, pluginType) {}
		#endregion
	}
}