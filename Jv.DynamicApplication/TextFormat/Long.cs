using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Long : AbstractNumber<long>
	{
		#region Constructors
		public Long() : base(long.TryParse) { }
		public Long(IEnumerable<char> input) : base(input, long.TryParse) { }

		public static implicit operator Long(string input)
		{
			return new Long(input);
		}

		public static implicit operator Long(long value)
		{
			return new Long(value.ToString());
		}
		#endregion
	}
}
