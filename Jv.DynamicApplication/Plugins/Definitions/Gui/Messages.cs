using System;
using System.Threading;
using System.Windows.Controls;
using Jv.DynamicApplication;
using Jv.Threading;

namespace Messages.Gui
{
	#region Ask
	public class AskInput : ITextInput
	{
		#region Constructors
		public AskInput(string text, AbstractInput input, ParameterMethod<AbstractInput> ok, ThreadStart cancel)
		{
			Text = text;
			Input = input;

			OkMethod = new CallBack<AbstractInput>(ok);
			CancelMethod = new CallBack(cancel);
		}
		#endregion

		#region Private
		AbstractInput Input { get; set; }

		CallBack<AbstractInput> OkMethod { get; set; }
		CallBack CancelMethod { get; set; }
		#endregion

		#region Public
		public bool AllowUserInput { get { return Input != null; } }

		public string Text { get; private set; }

		public bool Accept(char ch)
		{
			return AllowUserInput && Input.Accept(ch);
		}

		public bool Validate()
		{
			return AllowUserInput && Input.Validate();
		}

		public string UserInput
		{
			get { return AllowUserInput ? Input.Text : string.Empty; }
		}

		public string FormatedUserInput
		{
			get { return AllowUserInput ? Input.FormatedText : string.Empty; }
		}

		public bool Confirm()
		{
			if (!AllowUserInput || Input.Validate())
			{
				OkMethod.Execute(Input);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			if (AllowUserInput)
				Input.Clear();
		}

		public void Cancel()
		{
			CancelMethod.Execute();
		}
		#endregion
	}

	public class Ask : AskInput
	{
		#region Constructors
		public Ask(string text)
			: this(text, null, null) { }

		public Ask(string text, ThreadStart ok)
			: this(text, ok, null) { }

		public Ask(string text, ThreadStart ok, ThreadStart cancel)
			: base(text, null, WrapMethod(ok), cancel) { }
		#endregion

		static ParameterMethod<AbstractInput> WrapMethod(ThreadStart method)
		{
			if (method == null)
				return null;
			return input => method();
		}
	}

	public class Ask<InputType> : AskInput where InputType : AbstractInput
	{
		#region Constructors
		public Ask(string text, ParameterMethod<InputType> ok, ThreadStart cancel)
			: base(text, Activator.CreateInstance<InputType>(), WrapMethod(ok), cancel) { }
		#endregion

		static ParameterMethod<AbstractInput> WrapMethod(ParameterMethod<InputType> method)
		{
			if (method == null)
				return null;
			return input => method((InputType)input);
		}
	}
	#endregion

	#region Options
	public class DisplayOption : CallBack
	{
		#region Constructors
		public DisplayOption(string text, ThreadStart method)
			: base(method)
		{
			Text = text;
		}
		#endregion

		public string Text { get; private set; }
	}

	public class ClearOptions { }
	#endregion

	#region User Control
	public class DisplayUserControl
	{
		#region Constructors
		public DisplayUserControl(UserControl userControl)
		{
			UserControl = userControl;
		}
		#endregion

		public UserControl UserControl { get; private set; }
	}

	public class ClearUserControl { }
	#endregion
}
