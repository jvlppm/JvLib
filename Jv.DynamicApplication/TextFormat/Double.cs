using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Double : AbstractNumber<double>
	{
		#region Constructors
		public Double() : base(double.TryParse) { }
		public Double(IEnumerable<char> input) : base(input, double.TryParse) { }

		public static implicit operator Double(string input)
		{
			return new Double(input);
		}

		public static implicit operator Double(double value)
		{
			return new Double(value.ToString());
		}
		#endregion
	}
}
