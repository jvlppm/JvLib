using System.IO;

namespace Jv.Networking
{
	public static class WriteRawData
	{
		#region WriteData
		public static void WriteRawString(this BinaryWriter writer, string text)
		{
			foreach (char ch in text)
				writer.Write((byte)ch);
			writer.Write((byte)0x00);
		}
		#endregion
	}
}