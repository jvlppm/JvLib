using System.Windows;
using Jv.Plugins;
using Messages.Gui;

namespace Jv.DynamicApplication
{
	/// <summary>
	/// Base for all Gui Plugins (Graphical User Interface).
	/// Provides a way for the Business Plugins to interact with the User.
	/// </summary>
	public abstract class PGui : Plugin
	{
		#region Constructors
		protected PGui()
		{
			BindMessage<AskInput>(AskInput);
			BindMessage<DisplayOption>(DisplayOption);
			BindMessage<ClearOptions>(ClearOptions);
			BindMessage<DisplayUserControl>(DisplayUserControl);
			BindMessage<ClearUserControl>(ClearUserControl);
		}
		#endregion

		/// <summary>
		/// The Main Wpf Window that will be Shown at Startup.
		/// </summary>
		public abstract Window Window { get; }

		/// <summary>
		/// Show the user a message and asks for a input, then execute the Method Ok / Cancel in case the user Confirm / Abort.
		/// </summary>
		/// <param name="sender">Plugin sender of the message.</param>
		/// <param name="message">Message data.</param>
		protected abstract void AskInput(Plugin sender, AskInput message);

		/// <summary>
		/// Show the user a option in a Single-Level Menu Style.
		/// </summary>
		/// <param name="sender">Plugin sender of the message.</param>
		/// <param name="message">Message data.</param>
		protected abstract void DisplayOption(Plugin sender, DisplayOption message);

		/// <summary>
		/// Remove all the options in the Menu.
		/// </summary>
		/// <param name="sender">Plugin sender of the message.</param>
		/// <param name="message">Message data.</param>
		protected abstract void ClearOptions(Plugin sender, ClearOptions message);

		/// <summary>
		/// Show the user a Wpf UserControl.
		/// </summary>
		/// <param name="sender">Plugin sender of the message.</param>
		/// <param name="message">Message data.</param>
		protected abstract void DisplayUserControl(Plugin sender, DisplayUserControl message);

		/// <summary>
		/// Remove the visible Wpf UserControl.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		protected abstract void ClearUserControl(Plugin sender, ClearUserControl message);
	}
}
