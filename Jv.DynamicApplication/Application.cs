using System.Threading;
using Jv.Plugins;
using Jv.Plugins.Exceptions;
using Jv.Threading.Jobs;

namespace Jv.DynamicApplication
{
	public static class Application
	{
		public static Manager Plugins = new Manager();

		/// <summary>
		/// Start a Dynamic Application, using the Gui and Flux specified.
		/// </summary>
		/// <param name="window">Path to a .Net assembly that contains a PGui class.</param>
		/// <param name="flux">Path to a .Net assembly that contains a PFlux class.</param>
		public static void Run(string window, string flux)
		{
			try
			{
				System.Windows.Application app = new System.Windows.Application();

				// Load plugins.
				var Gui = Plugins.LoadPrivatePlugin<PGui>(window);
				var Flux = Plugins.LoadPlugin<PFlux>(flux);
				Plugins.LoadPlugin(new GuiChangeThread(Gui));

				// Show Interface.
				Gui.Window.Show();

				// Start Flux and Wait for the Gui to Close.
				bool fluxExited = false;
				Worker mainFlux = new Worker("Flux");
				mainFlux.Execute(Flux.Start);
				mainFlux.Execute(delegate
				{
					fluxExited = true;
					Gui.Window.Dispatcher.Invoke((ThreadStart)Gui.Window.Close);
				});
				mainFlux.Exit();

				app.Run();

				// Ensure the Flux is Stopped after Gui Close.
				if (!fluxExited)
					Flux.Exit();
			}
			catch (CouldNotLoad ex)
			{
				Plugins.MessageToPlugin<PLog>(ex);
				throw;
			}
		}
	}
}
