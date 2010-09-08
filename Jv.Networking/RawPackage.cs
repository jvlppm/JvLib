namespace Jv.Networking
{
	public class RawPackage
	{
		public int CurrentPosition { get; set; }

		public byte[] Data { get; protected set; }
		
		public RawPackage(byte[] data)
		{
			CurrentPosition = 0;
			Data = data;
		}

		public RawPackage(int size)
		{
			CurrentPosition = 0;
			Data = new byte[size];
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

		public string ReadString(params char[] endChars)
		{
			string text = string.Empty;
			char ch;
			bool finished;
			do
			{
				finished = false;
				ch = (char)ReadByte();
				foreach (var end in endChars)
				{
					if (ch == end)
					{
						finished = true;
						break;
					}
				}

				if (!finished)
					text += ch;
			} while (!finished && CurrentPosition < Data.Length);

			return text;
		}

		public byte[] ReadBytes(int count)
		{
			byte[] data = new byte[count];
			for(int i = 0; i < count; i++)
				data[i] = ReadByte();

			return data;
		}
		#endregion

		#region WriteData
		public void WriteByte(byte value)
		{
			Data[CurrentPosition++] = value;
		}
		public void WriteShort(short value)
		{
			WriteByte((byte)(value & 0xff));
			WriteByte((byte)(value >> 8));
		}
		public void WriteInt(int value)
		{
			WriteByte((byte)(value & 0xff));
			WriteByte((byte)((value >> 8) & 0xff));
			WriteByte((byte)((value >> 16) & 0xff));
			WriteByte((byte)(value >> 24));
		}
		public void WriteString(string text)
		{
			foreach (char ch in text)
				WriteByte((byte)ch);
			WriteByte(0x00);
		}
		public void WriteBytes(byte[] bytes)
		{
			bytes.CopyTo(Data, CurrentPosition);
			CurrentPosition += bytes.Length;
		}
		#endregion
	}
}
