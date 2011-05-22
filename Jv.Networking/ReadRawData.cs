using System.IO;
using System.Linq;
using System.Text;

namespace Jv.Networking
{
	public static class ReadRawData
	{
		public static bool EndOfData(this BinaryReader reader)
		{
			return reader.BaseStream.Position >= reader.BaseStream.Length;
		}

		public static string ReadRawString(this BinaryReader reader, params char[] endChars)
		{
			StringBuilder text = new StringBuilder();

			if (endChars.Length == 0)
				endChars = new []{ '\0' };

			while (!reader.EndOfData())
			{
				char ch = reader.ReadChar();
				if (endChars.Contains(ch))
					break;

				text.Append(ch);
			}

			return text.ToString();
		}

		public static string ReadRawString(this BinaryReader reader, int length)
		{
			StringBuilder text = new StringBuilder(length);

			for (int i = 0; i < length && !reader.EndOfData(); i++)
				text.Append(reader.ReadChar());

			return text.ToString();
		}
	}
}
