using System.Collections.Generic;

namespace Jv.Plugins
{
	class PList<PluginType> : List<PluginType>, ILearneable
	{
		#region Properties
		public bool Learned { get; set; }
		#endregion
	}

	interface ILearneable
	{
		bool Learned { get; set; }
	}
}