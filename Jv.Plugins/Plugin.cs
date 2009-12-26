using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Messages.Plugins;
using Jv.Plugins.Exceptions;
using Messages.Log;

namespace Jv.Plugins
{
	public abstract class Plugin
	{
		#region Nested types
		public delegate void Receive(Plugin sender, object message);
		public delegate void Receive<Type>(Plugin sender, Type message);
		public delegate void SimpleReceive(Plugin sender);
		protected delegate object ReturnFunction<InfoType>(Plugin sender, InfoType message);
		#endregion

		#region Attributes
		readonly Dictionary<Type, IList> _loadedPlugins;
		readonly Hashtable _messageBinds;
		object _lastMessage;
		Plugin _lastSender, _parent;
		bool _public;
		#endregion

		#region Constructors
		protected Plugin()
		{
			_loadedPlugins = new Dictionary<Type, IList>();
			_messageBinds = new Hashtable();

			Parent = null;
			_public = false;

			_lastMessage = null;
			_lastSender = null;
		}
		#endregion

		#region Protected Methods
		#region Virtual
		/// <summary>
		/// Chamado quando é lançada uma exceção ao receber uma mensagem.
		/// </summary>
		/// <param name="ex">Exception</param>
		protected internal virtual void OnException(Exception ex) { }

		/// <summary>
		/// Chamado após ser carregado por algum plugin.
		/// </summary>
		protected internal virtual void OnLoad() { }

		protected internal virtual void ReceiveMessage(Plugin sender, object message)
		{
			MessageToPlugin<PLog>(new LogObject("Received Unknown Message", message.GetType().Name));
		}
		#endregion

		#region Bind Messages
		/// <summary>
		/// Redireciona mensagens do tipo Type para a função callBack.
		/// Estas mensagens não serão enviadas para o ReceiveMessage.
		/// </summary>
		/// <typeparam name="Type">Tipo da mensagems a serem redirecionadas.</typeparam>
		/// <param name="callBack">Função que será chamada para mensagens do tipo Type.</param>
		protected void BindMessage<Type>(Receive<Type> callBack)
		{
			_messageBinds.Add(typeof(Type), CallReceive(callBack));
		}

		/// <summary>
		/// Redireciona mensagens do tipo Type para a função callBack.
		/// Estas mensagens não serão enviadas para o ReceiveMessage.
		/// </summary>
		/// <typeparam name="Type">Tipo das mensagens a serem redirecionadas.</typeparam>
		/// <param name="callBack">Função que será chamada para mensagens do tipo Type.</param>
		protected void BindMessage<Type>(SimpleReceive callBack)
		{
			_messageBinds.Add(typeof(Type), CallSimpleReceive(callBack));
		}

		/// <summary>
		/// Função será executada quando for chamado ValueFromPlugin com tipo InfoType.
		/// </summary>
		/// <typeparam name="InfoType"></typeparam>
		/// <param name="callBack"></param>
		protected void BindReturn<InfoType>(ReturnFunction<InfoType> callBack)
		{
			if (!_messageBinds.Contains(typeof(ReturnValue)))
				_messageBinds.Add(typeof(ReturnValue), new Hashtable());

			((Hashtable)_messageBinds[typeof(ReturnValue)]).Add(typeof(InfoType), CallReturnValue(callBack));
		}

		/// <summary>
		/// Remove redirecionamentos de mensagens do tipo Type.
		/// </summary>
		/// <typeparam name="Type"></typeparam>
		protected void UnBindMessage<Type>()
		{
			if (_messageBinds.ContainsKey(typeof(Type)))
				_messageBinds.Remove(typeof(Type));
		}
		#endregion

		#region Plugins Management
		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		protected internal PluginType LoadPlugin<PluginType>(string fileName) where PluginType : Plugin
		{
			PluginType obj = LoadOneFromFile<PluginType>(fileName);
			LoadPlugin(obj, true);

			return obj;
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="fileName">Arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		protected internal PluginType LoadPrivatePlugin<PluginType>(string fileName) where PluginType : Plugin
		{
			PluginType obj = LoadOneFromFile<PluginType>(fileName);
			LoadPlugin(obj, false);

			return obj;
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="assemblyData">Conteúdo de arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		protected internal PluginType LoadPlugin<PluginType>(byte[] assemblyData) where PluginType : Plugin
		{
			PluginType obj = LoadOneFromData<PluginType>(assemblyData);
			LoadPlugin(obj, true);

			return obj;
		}

		/// <summary>
		/// Carrega plugin do tipo PluginType.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin (Base).</typeparam>
		/// <param name="assemblyData">Conteúdo de arquivo (.dll) onde a classe do tipo PluginType está definida.</param>
		protected internal PluginType LoadPrivatePlugin<PluginType>(byte[] assemblyData) where PluginType : Plugin
		{
			PluginType obj = LoadOneFromData<PluginType>(assemblyData);
			LoadPlugin(obj, false);

			return obj;
		}

		/// <summary>
		/// Carrega Plugin interno. Mensagens para o plugin são enviadas para o seu tipo Base.
		/// </summary>
		/// <param name="plugin">Plugin instanciado.</param>
		protected internal void LoadPlugin(Plugin plugin)
		{
			LoadPlugin(plugin, true);
		}

		/// <summary>
		/// Carrega Plugin interno. Mensagens para o plugin são enviadas para o seu tipo Base.
		/// Plugin não estará acessível para outros plugins do mesmo nível.
		/// </summary>
		/// <param name="plugin">Plugin instanciado.</param>
		protected internal void LoadPrivatePlugin(Plugin plugin)
		{
			LoadPlugin(plugin, false);
		}

		/// <summary>
		/// Descarrega plugins do tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo dos Plugins (Base comum).</typeparam>
		protected internal void UnLoadPlugins<PluginType>() where PluginType : Plugin
		{
			if (!_loadedPlugins.ContainsKey(typeof(PluginType)))
				return;

			IList plugins = _loadedPlugins[typeof(PluginType)];

			ArrayList toUpdate = new ArrayList();
			toUpdate.AddRange(_loadedPlugins[typeof(Plugin)]);

			while (plugins.Count > 0)
			{
				if (_loadedPlugins.ContainsKey(typeof(Plugin)))
					ClearCache((Plugin)plugins[0], toUpdate);
				RemovePlugin((Plugin)plugins[0]);
			}
		}

		/// <summary>
		/// Descarrega plugin.
		/// </summary>
		/// <param name="plugin">Plugin a ser descarregado.</typeparam>
		protected internal void UnLoadPlugin(Plugin plugin)
		{
			ClearCache(plugin, _loadedPlugins[typeof(Plugin)]);
			RemovePlugin(plugin);
		}

		/// <summary>
		/// Acessa plugin carregado.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <exception cref="Plugins.Exceptions.MultipleInstances">Lançado quando mais de um Plugin do mesmo tipo foi carregado. Utilize GetPlugins.</exception>
		/// <exception cref="Plugins.Exceptions.PluginNotAccessible">Lançado quando Plugins do tipo não foram encontrados.</exception>
		protected internal PluginType GetPlugin<PluginType>() where PluginType : Plugin
		{
			PList<PluginType> plugins = GetPlugins<PluginType>(this);

			if (plugins.Count > 1)
				throw new MultipleInstances(typeof(PluginType));

			if (plugins.Count < 1)
				throw new PluginNotAccessible(typeof(PluginType));

			return plugins[0];
		}

		/// <summary>
		/// Acessa plugins carregados.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base dos plugins carregados.</typeparam>
		protected internal List<PluginType> GetPlugins<PluginType>() where PluginType : Plugin
		{
			return GetPlugins<PluginType>(this);
		}
		#endregion

		#region Send Message
		/// <summary>
		/// Envia mensagem para os plugins de tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <param name="message">Mensagem a ser recebida pelos plugins.</param>
		protected internal void MessageToPlugin<PluginType>(object message) where PluginType : Plugin
		{
			MessageToPlugin<PluginType>(this, message);
		}

		/// <summary>
		/// Envia mensagem para os plugins de tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		/// <param name="format">Formato da mensagem (string.Format).</param>
		/// <param name="args">argumentos da mensagem.</param>
		protected internal void MessageToPlugin<PluginType>(string format, params object[] args) where PluginType : Plugin
		{
			if (format == null)
				MessageToPlugin<PluginType>(this, null);
			else
				MessageToPlugin<PluginType>(this, string.Format(format, args));
		}

		/// <summary>
		/// Envia mensagem diretamente ao plugin destination.
		/// </summary>
		/// <param name="destination">Plugin que receberá a mensagem.</param>
		/// <param name="message">Mensagem a ser enviada.</param>
		protected internal void MessageToPlugin(Plugin destination, object message)
		{
			SendMessage(this, message, destination);
		}
		#endregion

		#region Return Value
		/// <summary>
		/// Realiza chamada de requisição de valor de um Plugin.
		/// </summary>
		/// <param name="destination">Plugin que contém a informação.</param>
		/// <param name="key">Objeto que representa a informação requisitada.</param>
		protected internal object ValueFromPlugin(Plugin destination, object key)
		{
			ReturnValue message = new ReturnValue(key);
			SendMessage(this, message, destination);
			return message.Value;
		}

		/// <summary>
		/// Realiza chamada de requisição de valor de um Plugin.
		/// </summary>
		/// <typeparam name="PluginType">Tipo do Plugin que contém a informação.</param>
		/// <param name="key">Objeto que representa a informação requisitada.</param>
		protected internal object ValueFromPlugin<PluginType>(object key) where PluginType : Plugin
		{
			ReturnValue message = new ReturnValue(key);
			SendMessage(this, message, GetPlugin<PluginType>());
			return message.Value;
		}
		#endregion

		#region ReSend Message
		/// <summary>
		/// Re-Envia mensagem recebida para os plugins de tipo PluginType.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		protected internal void ReSendMessage<PluginType>() where PluginType : Plugin
		{
			if (_lastSender == null)
				throw new NoMessageToResend();

			MessageToPlugin<PluginType>(_lastSender, _lastMessage);
		}

		/// <summary>
		/// Re-Envia conteudo da mensagem recebida para os plugins de tipo PluginType.
		/// Mensagem recebida deve ser um Messages.Plugins.PackedMessage.
		/// </summary>
		/// <typeparam name="PluginType">Tipo da Base do plugin carregado.</typeparam>
		protected internal void ReSendMessageContent<PluginType>() where PluginType : Plugin
		{
			if (_lastSender == null)
				throw new NoMessageToResend();

			if (!(_lastMessage is PackedMessage))
				throw new NoMessageToResend(
					string.Format("{0} is not a {1}. Only received messages of type {0} are supported for this operation",
								  _lastMessage.GetType().Name, typeof(PackedMessage).Name));

			MessageToPlugin<PluginType>(_lastSender, ((PackedMessage)_lastMessage).Content);
		}

		/// <summary>
		/// Re-Envia mensagem recebida para o plugin destination.
		/// </summary>
		/// <param name="destination">Plugin instanciado.</param>
		protected internal void ReSendMessage(Plugin destination)
		{
			if (_lastSender == null)
				throw new NoMessageToResend();

			SendMessage(_lastSender, _lastMessage, destination);
		}

		/// <summary>
		/// Re-Envia conteudo da mensagem recebida para o plugin destination.
		/// </summary>
		/// <param name="destination">Plugin instanciado.</param>
		protected internal void ReSendMessageContent(Plugin destination)
		{
			if (_lastSender == null)
				throw new NoMessageToResend();

			if (!(_lastMessage is PackedMessage))
				throw new NoMessageToResend(
					string.Format("{0} is not a {1}. Only received messages of type {0} are supported for this operation",
								  _lastMessage.GetType().Name, typeof(PackedMessage).Name));

			SendMessage(_lastSender, ((PackedMessage)_lastMessage).Content, destination);
		}
		#endregion
		#endregion

		#region Protected Properties
		/// <summary>
		/// Plugin "pai".
		/// Obs.: Válido apenas após o construtor.
		/// </summary>
		protected Plugin Parent
		{
			get { return _parent; }
			private set
			{
				if (_parent == null && value != null)
				{
					_parent = value;
					OnLoad();
				}
			}
		}
		#endregion

		#region Private Methods

		#region Bind Methods
		static Receive CallReceive<MessageType>(Receive<MessageType> real)
		{
			return (sender, message) => real(sender, (MessageType)message);
		}

		static Receive CallSimpleReceive(SimpleReceive real)
		{
			return (sender, message) => real(sender);
		}

		static Receive CallReturnValue<InfoType>(ReturnFunction<InfoType> real)
		{
			return
				delegate(Plugin sender, object message) { ((ReturnValue)message).Value = real(sender, (InfoType)((ReturnValue)message).Key); };
		}
		#endregion

		void LoadPlugin(Plugin plugin, bool isPublic)
		{
			if (plugin.Parent == null)
			{
				plugin._public = isPublic;
				plugin.Parent = this;
			}

			if (_loadedPlugins.ContainsKey(typeof(Plugin)))
				ClearCache(plugin, _loadedPlugins[typeof(Plugin)]);
			Type type = plugin.GetType();

			do
			{
				type = type.BaseType;

				if (_loadedPlugins.ContainsKey(type))
				{
					if (((ILearneable)_loadedPlugins[type]).Learned)
						_loadedPlugins[type].Clear();

					((ILearneable)_loadedPlugins[type]).Learned = false;
					_loadedPlugins[type].Add(plugin);
				}
				else
				{
					PList<Plugin> list = new PList<Plugin> { plugin };
					_loadedPlugins.Add(type, list);
				}
			} while (type != typeof(Plugin));
		}

		static PluginType LoadOneFromFile<PluginType>(string fileName) where PluginType : Plugin
		{
			Assembly pluginAssembly;

			try
			{
				AssemblyName an = AssemblyName.GetAssemblyName(fileName);
				pluginAssembly = Assembly.Load(an);
			}
			catch (Exception ex)
			{
				throw new CouldNotLoad(fileName, typeof(PluginType), ex);
			}

			return LoadOneFromAssembly<PluginType>(fileName, pluginAssembly);
		}

		static PluginType LoadOneFromData<PluginType>(byte[] data) where PluginType : Plugin
		{
			Assembly pluginAssembly;
			const string fileName = "InMemoryAssembly";

			try
			{
				pluginAssembly = Assembly.Load(data);
			}
			catch (Exception ex)
			{
				throw new CouldNotLoad(fileName, typeof(PluginType), ex);
			}

			return LoadOneFromAssembly<PluginType>(fileName, pluginAssembly);
		}

		static PluginType LoadOneFromAssembly<PluginType>(string fileName, Assembly pluginAssembly)
		{
			bool matched = false;
			Type chosen = null;
			int found = 0;

			// Percorre as classes da dll
			foreach (Type type in pluginAssembly.GetTypes())
			{
				// Verifica se a classe é do tipo buscado
				if (!type.IsAbstract &&
				    !type.IsNested &&
				    type.IsPublic &&
				    type.IsSubclassOf(typeof(PluginType)))
				{
					if (matched)
					{
						if (type.BaseType == typeof(PluginType))
							throw new ClassMultipleDefinition(fileName, typeof(PluginType));
					}
					else
					{
						if (type.BaseType == typeof(PluginType))
							matched = true;

						chosen = type;
						found++;
					}
				}
			}

			if (chosen == null)
				throw new ClassNotDefined(fileName, typeof(PluginType));

			if (!matched && found > 1)
				throw new ClassMultipleDefinition(fileName, typeof(PluginType));

			try
			{
				return (PluginType)Activator.CreateInstance(chosen);
			}
			catch (TargetInvocationException ex)
			{
				throw new CouldNotInstantiate(fileName, typeof(PluginType), ex.InnerException);
			}
			catch (Exception ex)
			{
				throw new CouldNotInstantiate(fileName, typeof(PluginType), ex);
			}
		}

		#region Remove Plugin
		void RemovePlugin(Plugin plugin)
		{
			Type type = plugin.GetType();
			do
			{
				type = type.BaseType;
				if (_loadedPlugins.ContainsKey(type))
				{
					_loadedPlugins[type].Remove(plugin);
					if (_loadedPlugins[type].Count == 0)
						_loadedPlugins.Remove(type);
				}
			} while (type != typeof(Plugin));
		}

		static void ClearCache(Plugin plugin, IList toUpdate)
		{
			if (!plugin._public)
				return;

			Type type = plugin.GetType();
			do
			{
				type = type.BaseType;
				ClearTypeCache(type, toUpdate);
			} while (type != typeof(Plugin));
		}

		static void ClearTypeCache(Type type, IList upUpdate)
		{
			if (upUpdate == null) return;

			foreach (Plugin loadedPlugin in upUpdate)
			{
				if (loadedPlugin._loadedPlugins.ContainsKey(type) &&
					((ILearneable)loadedPlugin._loadedPlugins[type]).Learned)
				{
					loadedPlugin._loadedPlugins.Remove(type);
					ClearTypeCache(type, loadedPlugin._loadedPlugins[typeof(Plugin)]);
				}
			}
		}
		#endregion

		#region Access to Plugins
		PList<PluginType> GetPlugins<PluginType>(Plugin caller) where PluginType : Plugin
		{
			PList<PluginType> found = new PList<PluginType> { Learned = true };

			// Nivel Acima (plugins carregados)
			if (_loadedPlugins.ContainsKey(typeof(PluginType)))
			{
				foreach (Plugin plugin in _loadedPlugins[typeof(PluginType)])
					if (plugin._public || (plugin.Parent == caller))
						found.Add((PluginType)plugin);
			}

			// Nivel Atual
			else if (this is PluginType)
				found.Add((PluginType)this);

			// Nivel Abaixo (plugins carregados pelo pai/avo)
			else if (Parent != null)
			{
				PList<PluginType> list = Parent.GetPlugins<PluginType>(caller);

				if (list.Count != 1 || list[0]._public)
				{
					/*list[0] == this._parent*/
					_loadedPlugins.Add(typeof(PluginType), list);
				}
				found.AddRange(list);
			}

			return found;
		}

		void MessageToPlugin<PluginType>(Plugin sender, object message) where PluginType : Plugin
		{
			List<PluginType> visiblePlugins = GetPlugins<PluginType>(sender);

			if (visiblePlugins.Count < 1)
				throw new PluginNotAccessible(typeof(PluginType));

			foreach (PluginType plugin in visiblePlugins)
				SendMessage(sender, message, plugin);
		}

		static void SendMessage(Plugin sender, object message, Plugin destination)
		{
			if (message == null)
				return;

			Plugin oldSender = destination._lastSender;
			object oldMessage = destination._lastMessage;

			try
			{
				destination._lastSender = sender;
				destination._lastMessage = message;
				bool notSent = true;
				Type messageType = message.GetType();

				if (messageType == typeof(ReturnValue))
				{
					if (destination._messageBinds.ContainsKey(messageType))
					{
						Type keyType = ((ReturnValue)message).Key.GetType();
						Hashtable regKeyTypes = (Hashtable)destination._messageBinds[messageType];

						do
						{
							if (regKeyTypes.Contains(keyType))
							{
								((Receive)regKeyTypes[keyType])(sender, message);
								notSent = false;
								break;
							}
							keyType = keyType.BaseType;
						} while (keyType != null);
					}
				}
				else
				{
					do
					{
						if (destination._messageBinds.ContainsKey(messageType))
						{
							((Receive)destination._messageBinds[messageType])(sender, message);
							notSent = false;
							break;
						}
						messageType = messageType.BaseType;
					} while (messageType != null);
				}

				if (notSent)
				{
					destination.ReceiveMessage(sender, message);
				}
			}
			catch (RemoteException ex)
			{
				sender.MessageToPlugin<PLog>("Exception: \"{0}\" when sending a message to {1}: Message: \"{2}\"", ex.Message,
											 destination.GetType().Name, message);

				if (ex.InnerException == null)
					throw new Exception(ex.Message);
				else
					throw ex.InnerException;
			}
			catch (Exception ex)
			{
				destination.MessageToPlugin<PLog>("Exception: \"{0}\" when receiving a message from {1}: Message: \"{2}\"",
												  ex.Message, sender.GetType().Name, message);
				destination.OnException(ex);
			}
			finally
			{
				destination._lastSender = oldSender;
				destination._lastMessage = oldMessage;
			}
		}
		#endregion

		#endregion
	}
}