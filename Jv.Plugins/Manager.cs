using System.Collections.Generic;

namespace Jv.Plugins
{
	public class Manager
	{
		#region Attributes
		readonly PluginManager _basePlugin;
		#endregion

		#region Constructors
		public Manager()
		{
			_basePlugin = new PluginManager();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		public PluginType LoadPlugin<PluginType>(string fileName) where PluginType : Plugin
		{
			return _basePlugin.LoadPlugin<PluginType>(fileName);
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		public PluginType LoadPrivatePlugin<PluginType>(string fileName) where PluginType : Plugin
		{
			return _basePlugin.LoadPrivatePlugin<PluginType>(fileName);
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		public PluginType LoadPlugin<PluginType>(byte[] data) where PluginType : Plugin
		{
			return _basePlugin.LoadPlugin<PluginType>(data);
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		public PluginType LoadPrivatePlugin<PluginType>(byte[] data) where PluginType : Plugin
		{
			return _basePlugin.LoadPrivatePlugin<PluginType>(data);
		}

		/// <summary>
		/// Carrega Plugin interno. Mensagens para o plugin são enviadas para o seu tipo Base.
		/// </summary>
		/// <param name="plugin">Plugin instanciado.</param>
		public void LoadPlugin(Plugin plugin)
		{
			_basePlugin.LoadPlugin(plugin);
		}

		/// <summary>
		/// Carrega Plugin interno. Mensagens para o plugin são enviadas para o seu tipo Base.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <param name="plugin">Plugin instanciado.</param>
		public void LoadPrivatePlugin(Plugin plugin)
		{
			_basePlugin.LoadPrivatePlugin(plugin);
		}

		/// <summary>
		/// Descarrega plugins do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo dos Plugins (Base comum).</typeparam>
		public void UnLoadPlugins<PluginType>() where PluginType : Plugin
		{
			_basePlugin.UnLoadPlugins<PluginType>();
		}

		/// <summary>
		/// Descarrega plugin.
		/// </summary>
		/// <param name="plugin">Plugin</typeparam>
		public void UnLoadPlugin(Plugin plugin)
		{
			_basePlugin.UnLoadPlugin(plugin);
		}

		/// <summary>
		/// Envia mensagem para os plugins de tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <param name="message">Mensagem a ser recebida pelos plugins.</param>
		public void MessageToPlugin<PluginType>(object message) where PluginType : Plugin
		{
			_basePlugin.MessageToPlugin<PluginType>(message);
		}

		/// <summary>
		/// Envia mensagem para os plugins de tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <param name="format">Formato da mensagem (string.Format).</param>
		/// <param name="args">argumentos da mensagem.</param>
		public void MessageToPlugin<PluginType>(string format, params object[] args) where PluginType : Plugin
		{
			_basePlugin.MessageToPlugin<PluginType>(format, args);
		}

		/// <summary>
		/// Envia mensagem diretamente ao plugin destination.
		/// </summary>
		/// <param name="destination">Plugin que receberá a mensagem.</param>
		/// <param name="message">Mensagem a ser enviada.</param>
		public void MessageToPlugin(Plugin destination, object message)
		{
			_basePlugin.MessageToPlugin(destination, message);
		}

		/// <summary>
		/// Acessa plugin carregado.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <exception cref="Plugins.Exceptions.MultipleInstances">Lançado quando mais de um Plugin do mesmo tipo foi carregado. Utilize GetPlugins.</exception>
		/// <exception cref="Plugins.Exceptions.PluginNotAccessible">Lançado quando Plugins do tipo não foram encontrados.</exception>
		public PluginType GetPlugin<PluginType>() where PluginType : Plugin
		{
			return _basePlugin.GetPlugin<PluginType>();
		}

		/// <summary>
		/// Acessa plugins carregados.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base dos plugins carregados.</typeparam>
		public List<PluginType> GetPlugins<PluginType>() where PluginType : Plugin
		{
			return _basePlugin.GetPlugins<PluginType>();
		}
		#endregion
	}
}