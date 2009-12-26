using System.Collections.Generic;
using System;

namespace Jv.DynamicApplication
{
	public interface ITextInput
	{
		bool Accept(char ch);
		bool Validate();
		void Clear();
	}

	public abstract class AbstractInput: ITextInput
	{
		#region Attributes
		readonly List<char> _acceptedChars;
		#endregion

		#region Constructors
		protected AbstractInput()
		{
			Text = string.Empty;
			_acceptedChars = new List<char>();
		}

		protected AbstractInput(IEnumerable<char> input)
			: this()
		{
			foreach (char ch in input)
			{
				if (!Accept(ch))
					throw new Exception("Input string was not in a correct format.");
			}
		}
		#endregion

		#region Public
		public abstract string Text { get; protected set; }
		public abstract string FormatedText { get; protected set; }

		public PreviewInputAddEventHandler PreviewInputAdd;
		public InputAddEventHandler InputAdd;
		public InputClearEventHandler InputClear;

		public bool Accept(char ch)
		{
			if (ch == '\b')
			{
				RemoveAll();
				if (_acceptedChars.Count > 0)
				{
					_acceptedChars.RemoveAt(_acceptedChars.Count - 1);
					foreach (char oldCh in _acceptedChars)
						Add(oldCh);

					return true;
				}
				return false;
			}

			if(PreviewInputAdd != null)
			{
				var eventArgs = new PreviewInputAddEventArgs(ch);
				PreviewInputAdd(this, eventArgs);
				if (eventArgs.Abort)
					return false;
			}

			if (Add(ch))
			{
				_acceptedChars.Add(ch);
				if (InputAdd != null)
					InputAdd(this, new InputAddEventArgs(ch));
				
				return true;
			}
			return false;
		}

		public abstract bool Validate();

		public void Clear()
		{
			_acceptedChars.Clear();
			RemoveAll();

			if(InputClear != null)
				InputClear(this, EventArgs.Empty);
		}
		#endregion

		protected abstract bool Add(char ch);
		protected abstract void RemoveAll();

		public override string ToString()
		{
			return FormatedText;
		}
	}
}
