using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Decimal : AbstractNumber<decimal>
	{
		#region Constructors
		public Decimal() : base(decimal.TryParse) { }
		public Decimal(IEnumerable<char> input) : base(input, decimal.TryParse) { }

		public static implicit operator Decimal(string input)
		{
			return new Decimal(input);
		}

		public static implicit operator Decimal(decimal value)
		{
			return new Decimal(value.ToString());
		}
		#endregion
	}
}
