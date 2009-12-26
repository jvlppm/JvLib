using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public class String : AbstractInput
	{
		#region Constructors
		public String() { }

		public String(IEnumerable<char> input) : base(input) { }

		public static implicit operator String(string input)
		{
			return new String(input);
		}
		#endregion

		#region Fields
		public override string Text { get; protected set; }
		public override string FormatedText
		{
			get { return Text; }
			protected set { Text = value; }
		}
		#endregion

		#region AbstractInput
		protected override bool Add(char ch)
		{
			Text += ch;
			return true;
		}

		public override bool Validate()
		{
			return true;
		}

		protected override void RemoveAll()
		{
			Text = string.Empty;
		}
		#endregion
	}
}
