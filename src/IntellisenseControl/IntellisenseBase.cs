using System;
using System.Windows.Forms;

namespace CnSharp.Windows.Forms.Editor.IntellisenseControl
{
	public abstract class IntellisenseBase
	{
        protected CodeEditor editor;

		protected Control intellisenseBox;

        public virtual CodeEditor Editor
		{
			set
			{
				this.editor = value;
				if (this.editor != null)
				{
					this.editor.ActiveTextAreaControl.TextArea.KeyDown += this.DoKeyDown;
					this.editor.ActiveTextAreaControl.TextArea.KeyUp += this.DoKeyUp;
					this.editor.ActiveTextAreaControl.TextArea.DoProcessDialogKey += this.DoProcessDialogKey;
					this.editor.ActiveTextAreaControl.VScrollBar.ValueChanged += this.ScrollBar_ValueChanged;
					this.editor.ActiveTextAreaControl.HScrollBar.ValueChanged += this.ScrollBar_ValueChanged;
					this.editor.ActiveTextAreaControl.TextArea.MouseClick += this.TextArea_MouseClick;
				}
			}
		}

        #region Methods

		protected static bool IsCharacterOrNumberKey(int keyValue)
		{
			return (keyValue >= 48 && keyValue <= 57) || (keyValue >= 65 && keyValue <= 90)
			       || (keyValue >= 96 && keyValue <= 105) || keyValue == 189;
		}

		protected virtual void DoKeyDown(object sender, KeyEventArgs e)
		{
		}

		protected virtual void DoKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Back)
			{
				if (this.intellisenseBox != null && this.intellisenseBox.Visible)
				{
					this.intellisenseBox.Hide();
				}
			}
		}


		private bool DoProcessDialogKey(Keys keyData)
		{
			if ((keyData == Keys.Enter || keyData == Keys.Tab || keyData == Keys.Up || keyData == Keys.Down
			     || keyData == Keys.Home || keyData == Keys.End)
			    && (this.intellisenseBox != null && this.intellisenseBox.Visible))
			{
				return true;
			}
			return false;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if (this.intellisenseBox != null && this.intellisenseBox.Visible)
			{
				this.intellisenseBox.Hide();
			}
		}

		private void TextArea_MouseClick(object sender, MouseEventArgs e)
		{
			if (this.intellisenseBox != null && this.intellisenseBox.Visible)
			{
				this.intellisenseBox.Hide();
			}
		}

		#endregion
	}
}