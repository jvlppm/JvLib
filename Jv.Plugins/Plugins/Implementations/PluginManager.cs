namespace Jv.Plugins
{
	class PluginManager : Plugin
	{
		#region Constructors
		public PluginManager()
		{
			LoadPlugin(new LogManager());
		}
		#endregion

		#region Plugin Implementation
		protected internal override void ReceiveMessage(Plugin sender, object message)
		{
			MessageToPlugin<PLog>("Received unkown message from {0}: {1}", sender, message);
		}
		#endregion
	}
}