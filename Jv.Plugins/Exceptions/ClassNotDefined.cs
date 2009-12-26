using System;

namespace Jv.Plugins.Exceptions
{
	public class ClassNotDefined : CouldNotLoad
	{
		#region Constructors
		public ClassNotDefined(string dllPath, Type pluginType)
			: base(dllPath, pluginType, string.Format("Class of type {0} is not defined in {1}.", pluginType.Name, dllPath)) { }
		#endregion
	}
}