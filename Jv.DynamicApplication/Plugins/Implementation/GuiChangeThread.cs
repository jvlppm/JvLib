using System.Threading;
using Jv.Plugins;
using Messages.Gui;

namespace Jv.DynamicApplication
{
	class GuiChangeThread : PGui
	{
		public PGui Gui { get; set; }

		#region Constructors
		public GuiChangeThread(PGui gui)
		{
			Gui = gui;
			UnBindMessage<DisplayOption>();
			UnBindMessage<ClearOptions>();
			UnBindMessage<DisplayUserControl>();
			UnBindMessage<ClearUserControl>();
			UnBindMessage<AskInput>();
		}
		#endregion

		public override System.Windows.Window Window
		{
			get { return Gui.Window; }
		}

		protected override void DisplayOption(Plugin sender, DisplayOption message) { }
		protected override void ClearOptions(Plugin sender, ClearOptions message) { }
		protected override void DisplayUserControl(Plugin sender, DisplayUserControl message) { }
		protected override void ClearUserControl(Plugin sender, ClearUserControl message) { }
		protected override void AskInput(Plugin sender, AskInput message) { }

		protected override void ReceiveMessage(Plugin sender, object message)
		{
			Window.Dispatcher.Invoke((ThreadStart) (() => ReSendMessage(Gui)));
		}
	}
}
