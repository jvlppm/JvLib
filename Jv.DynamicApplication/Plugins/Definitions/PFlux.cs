using System;

namespace Jv.DynamicApplication
{
	/// <summary>
	/// Base for the main Flux Plugin (Business).
	/// A State-Machine that represents the main Flux of the Application.
	/// </summary>
	public abstract class PFlux : PBusiness
	{
		bool Running { get; set; }

		new public void Start()
		{
			Running = true;

			int lastWorker = Workers.Count;
			base.Start();
			while (Workers.Count > lastWorker)
			{
				Workers[lastWorker].Exit();
				Workers.RemoveAt(lastWorker);
			}
		}

		new public void Exit()
		{
			if (!Running)
				throw new Exception("Exit must be called in a Started Flux");
			Running = false;
			base.Exit();
		}
	}
}
