using System.Collections.Generic;

namespace Jv.DynamicApplication
{
	public delegate bool ParseMethod<ParseType>(string input, out ParseType result);

	public abstract class AbstractNumber<SystemType> : AbstractInput
	{
		#region Constructors
		protected AbstractNumber(ParseMethod<SystemType> parser, bool acceptNegative)
		{
			AcceptNegative = acceptNegative;
			TryParse = parser;
		}

		protected AbstractNumber(ParseMethod<SystemType> parser)
			: this(parser, true) { }

		protected AbstractNumber(IEnumerable<char> input, ParseMethod<SystemType> parser)
			: this(input, parser, true) { }

		protected AbstractNumber(IEnumerable<char> input, ParseMethod<SystemType> parser, bool acceptNegative)
			: this(parser, acceptNegative)
		{
			foreach (char ch in input)
			{
				if (!Accept(ch))
					throw new System.Exception("Input string was not in a correct format.");
			}
		}
		#endregion

		#region Fields
		public bool AcceptNegative { get; private set; }
		readonly ParseMethod<SystemType> TryParse;

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
			if (AcceptNegative && string.IsNullOrEmpty(Text) && ch == '-')
			{
				Text += ch;
				return true;
			}

			SystemType result;
			if (TryParse(Text + ch, out result))
			{
				Text += ch;
				return true;
			}
			return false;
		}

		public override bool Validate()
		{
			SystemType result;
			return TryParse(Text, out result);
		}

		protected override void RemoveAll()
		{
			Text = string.Empty;
		}
		#endregion

		#region Data Access
		public SystemType Value
		{
			get
			{
				SystemType result;
				if (!TryParse(Text, out result))
					throw new System.Exception("Input string was not in a correct format.");
				return result;
			}
		}
		#endregion
	}
}
