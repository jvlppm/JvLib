using System.Collections.Generic;

namespace Jv.Networking
{
	public class WriteRawData
	{
		public int CurrentPosition { get { return Data.Count; } }

		public List<byte> Data { get; protected set; }

		public WriteRawData()
		{
			Data = new List<byte>();
		}

		#region WriteData
		public void WriteByte(byte value)
		{
			Data.Add(value);
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
			Data.AddRange(bytes);
		}
		#endregion
	}
}