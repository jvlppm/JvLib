using System;

namespace Jv.DynamicApplication
{
	public class BusinessException : Exception
	{
		#region Constructors
		public BusinessException(string message)
			: base(message) { }
		#endregion
	}

	public class AbortedException : BusinessException
	{
		#region Constructors
		public AbortedException(string message)
			: base(message) { }
		#endregion
	}
}
