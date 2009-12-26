namespace Messages.Plugins
{
	public class PackedMessage
	{
		#region Fields
		public object Content { get; private set; }
		#endregion

		#region Constructors
		public PackedMessage(object packedMessage)
		{
			Content = packedMessage;
		}
		#endregion
	}

	class ReturnValue
	{
		#region Constructors
		public ReturnValue()
		{
			Key = null;
		}

		public ReturnValue(object key)
		{
			Key = key;
		}
		#endregion

		#region Properties
		public object Key { get; set; }
		public object Value { get; set; }
		#endregion
	}
}