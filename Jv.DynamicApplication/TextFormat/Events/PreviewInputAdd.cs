namespace Jv.DynamicApplication
{
	public class PreviewInputAddEventArgs : InputAddEventArgs
	{
		public bool Abort { get; set; }

		#region Constructors
		public PreviewInputAddEventArgs(char ch)
			: base(ch)
		{
			Abort = false;
		}
		#endregion
	}

	public delegate void PreviewInputAddEventHandler(AbstractInput sender, PreviewInputAddEventArgs e);
}
