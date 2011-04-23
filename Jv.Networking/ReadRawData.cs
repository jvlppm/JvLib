using System.Linq;
using System.Text;

namespace Jv.Networking
{
	public class ReadRawData
	{
		public int CurrentPosition { get; set; }

		public byte[] Data { get; protected set; }

		public ReadRawData(byte[] data)
		{
			CurrentPosition = 0;
			Data = data;
		}

		public bool EndOfData { get { return CurrentPosition >= Data.Length; } }

		#region Read Data
		public byte ReadByte()
		{
			return Data[CurrentPosition++];
		}

		public short ReadShort()
		{
			return (short)((ReadByte()) + (((uint)ReadByte()) << 8));
		}

		public int ReadInt()
		{
			return (int)((ReadByte()) + (((uint)ReadByte()) << 8) + (((uint)ReadByte()) << 16) + (((uint)ReadByte()) << 24));
		}

		public string ReadString()
		{
			return ReadString('\0');
		}

		public string ReadString(int length)
		{
			StringBuilder text = new StringBuilder(length);

			for(int i = 0; i < length && CurrentPosition < Data.Length; i++)
				text.Append(ReadChar());

			return text.ToString();
		}

		public string ReadString(params char[] endChars)
		{
			StringBuilder text = new StringBuilder();

			while (CurrentPosition < Data.Length)
			{
				char ch = ReadChar();
				if (endChars.Contains(ch))
					break;

				text.Append(ch);
			}

			return text.ToString();
		}

		public char ReadChar()
		{
			return (char)ReadByte();
		}

		public byte[] ReadBytes(int count)
		{
			byte[] data = new byte[count];
			for(int i = 0; i < count; i++)
				data[i] = ReadByte();

			return data;
		}
		#endregion
	}
}
