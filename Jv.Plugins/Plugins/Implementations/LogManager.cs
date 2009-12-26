using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Jv.Plugins.Exceptions;
using Messages.Log;

namespace Jv.Plugins
{
	class LogManager : PLog
	{
		#region Attributes
		readonly Dictionary<Plugin, StreamWriter> _files;
		readonly Dictionary<Plugin, Stack<StartGroup>> _structure;
		#endregion

		#region Constructors
		public LogManager()
		{
			_files = new Dictionary<Plugin, StreamWriter>();
			_structure = new Dictionary<Plugin, Stack<StartGroup>>();
			Path = "Logs";

			DirectoryInfo directoryInfo = new DirectoryInfo(Path);

			if (directoryInfo.Exists)
			{
				foreach (var file in directoryInfo.GetFiles())
				{
					try { File.Delete(file.FullName); }
					catch { }
				}
			}
			else directoryInfo.Create();

			_files.Add(this, File.CreateText(string.Format("{0}/Global.txt", Path)));
		}
		#endregion

		#region PLog Implementation

		public string Path { get; set; }

		protected override void LogString(Plugin sender, string message)
		{
			if (message.Contains("\r\n"))
			{
				foreach (string subString in message.Replace("\r\n", "\n").Split('\n'))
					LogString(sender, subString);
				return;
			}

			string dados = string.Format("{0} {1,-10}: {2}{3}", DateTime.Now.ToString("HH:mm:ss.fff"), sender.GetType().Name,
											 CalculateTabs(sender), message);

			WriteLog(sender, dados);
		}

		protected override void LogException(Plugin sender, Exception ex)
		{
			LogMessageGroup(sender, ReadException(string.Format("Exception: {0}", ex.GetType().Name), ex));
		}

		protected internal override void ReceiveMessage(Plugin sender, object message)
		{
			LogObject(sender, new LogObject(message.GetType().Name, message));
		}

		protected override void LogObject(Plugin sender, LogObject message)
		{
			LogMessageGroup(sender, ReadObject(message.Name, message.Value));
		}

		protected override void StartGroup(Plugin sender, StartGroup message)
		{
			LogString(sender, message.Name);

			if (!_structure.ContainsKey(sender))
				_structure.Add(sender, new Stack<StartGroup>());

			_structure[sender].Push(message);
		}

		protected override void EndGroup(Plugin sender, EndGroup message)
		{
			if (!_structure.ContainsKey(sender))
				throw new RemoteException("No group was started.");

			while (_structure[sender].Count > 0)
			{
				StartGroup group = _structure[sender].Pop();

				if (message.Name == null || group.Name == message.Name)
					break;
			}

			if (_structure[sender].Count == 0)
				_structure.Remove(sender);
		}

		protected override void LogMessageGroup(Plugin sender, MessageGroup messageGroup)
		{
			StartGroup(sender, new StartGroup(messageGroup.Message));

			if (messageGroup.Count > 0)
			{
				foreach (MessageGroup subGroup in messageGroup)
				{
					LogMessageGroup(sender, subGroup);
				}
			}

			EndGroup(sender, new EndGroup());
		}
		#endregion

		#region Private Methods
		public static MessageGroup ReadObject(string name, object obj)
		{
			return InternalReadObject(name, obj, new List<object>());
		}

		static MessageGroup InternalReadObject(string name, object obj, ICollection<object> loggedObjects)
		{
			if (obj != null && !obj.GetType().IsValueType && loggedObjects.Contains(obj))
				return new MessageGroup(string.Format("{0}: <recusive>", name));
			
			loggedObjects.Add(obj);

			MessageGroup message;
			if (obj == null)
			{
				message = new MessageGroup(string.Format("{0}: null", name));
			}

			else if (typeof(IEnumerable).IsAssignableFrom(obj.GetType()) && obj.GetType() != typeof(string))
			{
				int i = 0;
				message = new MessageGroup(name);
				IEnumerator enumerator = ((IEnumerable)obj).GetEnumerator();
				if (enumerator != null)
				{
					foreach (object item in ((IEnumerable)obj))
					{
						message.Add(InternalReadObject((i++).ToString(), item, loggedObjects));
					}
				}
			}

			else if (obj.GetType().GetProperties().Length > 0 && obj.GetType() != typeof(string))
			{
				message = new MessageGroup(name);
				foreach (PropertyInfo pInfo in obj.GetType().GetProperties())
				{
					if (!pInfo.GetGetMethod().IsPublic)
						continue;

					object subObject;

					try { subObject = pInfo.GetValue(obj, null); }
					catch { continue; }

					message.Add(InternalReadObject(pInfo.Name, subObject, loggedObjects));
				}
			}
			else
			{
				message = new MessageGroup(string.Format("{0}: {1}", name, obj));
			}
			return message;
		}

		public static MessageGroup ReadException(string name, Exception message)
		{
			if (message == null)
				return null;
			MessageGroup group = new MessageGroup(name);

			if (!string.IsNullOrEmpty(message.Message))
				group.Add(string.Format("Message: {0}", message.Message));

			//if (!string.IsNullOrEmpty(message.StackTrace))
			//	group.Add(string.Format("StackTrace: {0}", message.StackTrace));

			if (message.InnerException != null)
				group.Add(ReadException(string.Format("InnerException: {0}", message.InnerException.GetType().Name), message.InnerException));

			return group;
		}

		string CalculateTabs(Plugin sender)
		{
			return _structure.ContainsKey(sender) ? new string('\t', _structure[sender].Count) : string.Empty;
		}

		void WriteLog(Plugin sender, string text)
		{
			try
			{
				lock (sender)
				{
					if (!_files.ContainsKey(sender))
						_files.Add(sender, File.CreateText(string.Format("{0}/{1}.txt", Path, sender)));

				
					_files[sender].WriteLine(text);
					_files[sender].Flush();
				}
			}
			finally
			{
				lock (this)
				{
					_files[this].WriteLine(text);
					_files[this].Flush();
				}
			}
		}
		#endregion
	}
}