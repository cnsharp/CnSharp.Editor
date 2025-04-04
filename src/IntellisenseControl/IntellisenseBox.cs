using System.Drawing;
using System.Windows.Forms;

namespace CnSharp.Windows.Forms.Editor.IntellisenseControl
{
	public class IntellisenseBox : ListBox
	{
        private ImageList imageList;

        public IntellisenseBox()
		{
			// Set owner draw mode
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.BorderStyle = BorderStyle.FixedSingle;
			this.Font = new Font("Arial", 9F);
		}

        public ImageList ImageList
		{
			get => this.imageList;
            set
			{
				this.imageList = value;
				if (this.imageList != null)
				{
					this.ItemHeight = this.imageList.ImageSize.Height + 2;
					this.Height = this.ItemHeight * 10 + 2;
				}
			}
		}

        public void AdjustSize()
		{
			int maxWidth = 0;
			foreach (object item in this.Items)
			{
				var width = (int)(item.ToString().Length * this.Font.Size);
				if (maxWidth < width)
				{
					maxWidth = width;
				}
			}
			maxWidth = (maxWidth + this.imageList.ImageSize.Width + 30);
			this.Width = maxWidth > 120 ? maxWidth : 120;
		}

        protected override void OnDrawItem(DrawItemEventArgs e)
		{
			//if (DesignMode)
			//    return;
			e.DrawBackground();
			e.DrawFocusRectangle();
			IntellisenseBoxItem item;
			Rectangle bounds = e.Bounds;
			Size imageSize = this.imageList.ImageSize;
			try
			{
				item = (IntellisenseBoxItem)this.Items[e.Index];
				if (item.ImageIndex != -1)
				{
					if (e.Index == this.SelectedIndex) //��ͼ��ı���ɫ����Ϊ��ɫ
					{
						Brush b = new SolidBrush(this.BackColor);
						e.Graphics.FillRectangle(b, bounds.Left, bounds.Top, imageSize.Width, this.ItemHeight);
					}
					this.imageList.Draw(e.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
				}
				//else
				//{
				//    e.Graphics.DrawString(item.Text, e.Font,new SolidBrush(e.ForeColor),
				//        bounds.Left, bounds.Top);
				//}

				e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor), bounds.Left + imageSize.Width, bounds.Top);
			}
			catch
			{
				if (e.Index != -1)
				{
					e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
				}
				else
				{
					e.Graphics.DrawString(this.Text, e.Font, new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
				}
			}

			base.OnDrawItem(e);
		}
    }
}