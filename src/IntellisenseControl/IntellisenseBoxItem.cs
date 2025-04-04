namespace CnSharp.Windows.Forms.Editor.IntellisenseControl
{
	public class IntellisenseBoxItem
	{
		#region Constructors and Destructors

		public IntellisenseBoxItem(string text, string tooltipText, int imageIndex)
		{
			this.Text = text;
			this.TooltipText = tooltipText;
			this.ImageIndex = imageIndex;
		}

		public IntellisenseBoxItem(string text, string tooltipText, string imageKey)
		{
			this.Text = text;
			this.TooltipText = tooltipText;
			this.ImageKey = imageKey;
		}

		public IntellisenseBoxItem(string text, int imageIndex)
			: this(text, string.Empty, imageIndex)
		{
		}

		public IntellisenseBoxItem(string text, string tooltipText)
			: this(text, tooltipText, -1)
		{
		}

		public IntellisenseBoxItem(string text)
			: this(text, string.Empty, -1)
		{
		}

		public IntellisenseBoxItem()
			: this(string.Empty)
		{
		}

		#endregion

		#region Public Properties

		public int ImageIndex { get; set; }

		public string ImageKey { get; set; }

		public object Tag { get; set; }

		public string Text { get; set; }

		public string TooltipText { get; set; }

		#endregion

        public override string ToString()
		{
			return this.Text;
		}
    }
}