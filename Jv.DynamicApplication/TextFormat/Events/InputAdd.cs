using System;

namespace Jv.DynamicApplication
{
	public class InputAddEventArgs : EventArgs
	{
		public char Char { get; private set; }

		#region Constructors
		public InputAddEventArgs(char ch)
		{
			Char = ch;
		}
		#endregion
	}

	public delegate void InputAddEventHandler(AbstractInput sender, InputAddEventArgs e);
}
