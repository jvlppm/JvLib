using System;
using System.Collections.Generic;
using System.Collections;
using Messages.Log;

namespace Jv.Plugins
{
	public abstract class PLog : Plugin
	{
		#region Constructors
		protected PLog()
		{
			BindMessage<string>(LogString);
			BindMessage<Exception>(LogException);

			BindMessage<StartGroup>(StartGroup);
			BindMessage<EndGroup>(EndGroup);
			BindMessage<MessageGroup>(LogMessageGroup);
			BindMessage<LogObject>(LogObject);
		}
		#endregion

		#region Abstract
		protected abstract void LogString(Plugin sender, string message);
		protected abstract void LogException(Plugin sender, Exception ex);
		protected abstract void StartGroup(Plugin sender, StartGroup message);
		protected abstract void EndGroup(Plugin sender, EndGroup message);
		protected abstract void LogMessageGroup(Plugin sender, MessageGroup messageGroup);

		protected abstract void LogObject(Plugin sender, LogObject message);
		#endregion
	}
}

namespace Messages.Log
{
	public class StartGroup
	{
		public StartGroup(string name)
		{
			Name = name;
		}
		public string Name { get; private set; }
	}

	public class EndGroup
	{
		public EndGroup() { }
		public EndGroup(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

	public class MessageGroup : ICollection<MessageGroup>
	{
		public MessageGroup(string message)
		{
			Message = message;
		}

		public string Message { get; private set; }
		List<MessageGroup> SubGroups { get; set; }

		#region ICollection

		public void Add(MessageGroup group)
		{
			if (SubGroups == null)
				SubGroups = new List<MessageGroup>();

			if (group != null)
				SubGroups.Add(group);
		}

		public void Add(string message)
		{
			Add(new MessageGroup(message));
		}

		#region IEnumerable<MessageGroup> Members

		public IEnumerator<MessageGroup> GetEnumerator()
		{
			if (SubGroups == null)
				return EmptyEnumerator<MessageGroup>();
			return SubGroups.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (SubGroups == null)
			{
				yield return Message;
				yield break;
			}
			foreach (MessageGroup subGroup in SubGroups)
				yield return subGroup;
		}

		static IEnumerator EmptyEnumerator() { yield break; }
		static IEnumerator<Type> EmptyEnumerator<Type>() { yield break; }

		#endregion

		public void Clear()
		{
			SubGroups.Clear();
		}

		public bool Contains(MessageGroup item)
		{
			if (SubGroups.Contains(item))
				return true;

			bool found = false;

			foreach (MessageGroup group in SubGroups)
			{
				if (group.Contains(item))
				{
					found = true;
					break;
				}
			}

			return found;
		}

		public void CopyTo(MessageGroup[] array, int arrayIndex)
		{
			array[arrayIndex] = this;
		}

		public int Count
		{
			get { return SubGroups != null ? SubGroups.Count : 0; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(MessageGroup item)
		{
			if (SubGroups.Contains(item))
			{
				SubGroups.Remove(item);
				return true;
			}

			bool found = false;

			foreach (MessageGroup group in SubGroups)
			{
				if (group.Remove(item))
				{
					found = true;
					break;
				}
			}

			return found;
		}

		#endregion
	}

	public class LogObject
	{
		public LogObject(string name, object value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; private set; }
		public object Value { get; private set; }
	}
}
