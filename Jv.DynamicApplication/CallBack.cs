using System;
using System.Threading;
using Jv.Threading;
using Jv.Threading.Jobs;

namespace Jv.DynamicApplication
{
	public class CallBack
	{
		#region Constructors
		public CallBack(ThreadStart method)
		{
			Caller = Worker.CurrentWorker;
			if (Caller == null)
				throw new Exception("CallBack can only be created by a Asynchronous Worker.");

			Method = method;
		}
		#endregion

		Worker Caller { get; set; }
		ThreadStart Method { get; set; }

		public void Execute()
		{
			Caller.Execute(Method);
		}
	}

	public class CallBack<Type>
	{
		#region Constructors
		public CallBack(ParameterMethod<Type> method)
		{
			Caller = Worker.CurrentWorker;
			if (Caller == null)
				throw new Exception("ParamCallBack can only be created by a Asynchronous Worker.");

			Method = method;
		}
		#endregion

		Worker Caller { get; set; }
		ParameterMethod<Type> Method { get; set; }

		public void Execute(Type parameter)
		{
			Caller.Execute(parameter, Method);
		}
	}
}
