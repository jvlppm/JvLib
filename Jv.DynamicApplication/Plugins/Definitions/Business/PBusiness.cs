using System.Collections.Generic;
using System.Threading;
using Jv.Plugins;
using Jv.Threading;
using Jv.Threading.Jobs;

namespace Jv.DynamicApplication
{
	/// <summary>
	/// Base for all Business Plugins.
	/// A State-Machine that represents a specific function in the Application.
	/// </summary>
	public abstract class PBusinessBase : Plugin
	{
		#region Static
		static int lastWorker;
		static public readonly List<Worker> Workers = new List<Worker>();
		internal static Worker GetWorker()
		{
			if (Workers.Count <= lastWorker)
				Workers.Add(new Worker(string.Format("Stack {0}", lastWorker.ToString("00"))));

			return Workers[lastWorker++];
		}
		internal static void ReleaseWorker()
		{
			lastWorker--;
		}
		#endregion

		#region Attributes
		internal AbortedException Aborted { get; set; }
		internal Semaphore Waiter { get; private set; }
		#endregion

		#region Constructors
		protected PBusinessBase()
		{
			Waiter = new Semaphore(0, 1);
		}
		#endregion

		internal void Wait()
		{
			Aborted = null;
			Waiter.WaitOne();

			if (Aborted != null)
				throw Aborted;
		}

		internal void Exit()
		{
			ReleaseWorker();
			Waiter.Release();
		}

		public void Abort(string why)
		{
			Aborted = new AbortedException(why);
			Waiter.Release();
		}

		#region Protected Methods
		protected void UpdateGui(ThreadStart method)
		{
			GetPlugin<PGui>().Window.Dispatcher.BeginInvoke(method);
		}

		protected void UpdateGui<ParameterType>(ParameterType param, ParameterMethod<ParameterType> method)
		{
			GetPlugin<PGui>().Window.Dispatcher.BeginInvoke(method, param);
		}
		#endregion
	}

	/// <summary>
	/// A Business Plugin that have no entrace/result.
	/// </summary>
	public abstract class PBusiness : PBusinessBase
	{
		public void Start()
		{
			GetWorker().Execute(Startup);
			Wait();
		}

		protected abstract void Startup();

		new protected void Exit()
		{
			base.Exit();
		}
	}

	/// <summary>
	/// A Business Plugin that have a entrace but no result.
	/// </summary>
	public abstract class PBusinessI<InputType> : PBusinessBase
	{
		public void Start(InputType entrance)
		{
			GetWorker().Execute(new Job<InputType>(entrance, Startup));
			Wait();
		}

		protected abstract void Startup(InputType entrance);

		new protected void Exit()
		{
			base.Exit();
		}
	}

	/// <summary>
	/// A Business Plugin that do not have a entrace but result.
	/// </summary>
	public abstract class PBusinessO<OutputType> : PBusinessBase
	{
		#region Attributes
		OutputType _result;
		#endregion

		public OutputType Start()
		{
			GetWorker().Execute(Startup);
			return Result;
		}

		protected abstract void Startup();

		protected OutputType Result
		{
			get
			{
				Wait();
				return _result;
			}
			set
			{
				_result = value;
				Exit();
			}
		}
	}

	/// <summary>
	/// A Business Plugin that have both entrace and result.
	/// </summary>
	public abstract class PBusinessIO<InputType, OutputType> : PBusinessBase
	{
		#region Attributes
		OutputType _result;
		#endregion

		public OutputType Start(InputType entrance)
		{
			GetWorker().Execute(new Job<InputType>(entrance, Startup));
			return Result;
		}

		protected abstract void Startup(InputType entrance);

		protected OutputType Result
		{
			get
			{
				Wait();
				return _result;
			}
			set
			{
				_result = value;
				Exit();
			}
		}
	}
}
