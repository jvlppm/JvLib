using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class Float : AbstractNumber<float>
	{
		#region Constructors
		public Float() : base(float.TryParse) { }
		public Float(IEnumerable<char> input) : base(input, float.TryParse) { }

		public static implicit operator Float(string input)
		{
			return new Float(input);
		}

		public static implicit operator Float(float value)
		{
			return new Float(value.ToString());
		}
		#endregion
	}
}
