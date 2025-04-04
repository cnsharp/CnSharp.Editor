using System;
using System.IO;
using CnSharp.Windows.Forms.Frame;

namespace CnSharp.Windows.Forms.Editor
{
	public partial class CodeWindow : GuardedTabWindow
	{
		#region Constructors and Destructors

		public CodeWindow()
		{
			this.InitializeComponent();
			this.codeEditor1.TextChanged += this.codeEditor1_TextChanged;
		}

		#endregion

		#region Public Properties

		public new CodeEditor Editor
		{
			get
			{
				return this.codeEditor1;
			}
		}

		#endregion

		#region Methods

		protected override void SaveFile()
		{
			if (this.Modified && !string.IsNullOrEmpty(this.codeEditor1.FileName))
			{
				File.WriteAllText(this.codeEditor1.FileName, this.codeEditor1.Text);
			}
		}

		private void codeEditor1_TextChanged(object sender, EventArgs e)
		{
			this.Modified = this.codeEditor1.Modified;
		}

		#endregion
	}
}