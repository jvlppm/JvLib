using System.Linq;
using System.Text;
using System;

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
			var value = BitConverter.ToInt16(Data, CurrentPosition);
			CurrentPosition += 2;
			return value;
		}

		public int ReadInt()
		{
			var value = BitConverter.ToInt32(Data, CurrentPosition);
			CurrentPosition += 4;
			return value;
		}

		public float ReadFloat()
		{
			var value = BitConverter.ToSingle(Data, CurrentPosition);
			CurrentPosition += 4;
			return value;
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
			Array.Copy(Data, CurrentPosition, data, 0, count);
			CurrentPosition += count;
			return data;
		}
		#endregion
	}
}
