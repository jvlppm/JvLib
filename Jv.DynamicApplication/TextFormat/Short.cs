using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Short : AbstractNumber<short>
	{
		#region Constructors
		public Short() : base(short.TryParse) { }
		public Short(IEnumerable<char> input) : base(input, short.TryParse) { }

		public static implicit operator Short(string input)
		{
			return new Short(input);
		}

		public static implicit operator Short(short value)
		{
			return new Short(value.ToString());
		}
		#endregion
	}
}
