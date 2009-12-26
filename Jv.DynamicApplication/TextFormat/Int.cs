using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Int : AbstractNumber<int>
	{
		#region Constructors
		public Int() : base(int.TryParse) { }
		public Int(IEnumerable<char> input) : base(input, int.TryParse) { }

		public static implicit operator Int(string input)
		{
			return new Int(input);
		}

		public static implicit operator Int(int value)
		{
			return new Int(value.ToString());
		}
		#endregion
	}
}
