using System.Drawing;
using System.Windows.Forms;

namespace CnSharp.Windows.Forms.Editor
{
	public class TipLabel : Label
	{
		public TipLabel()
		{
			BackColor = SystemColors.Info;
			BorderStyle = BorderStyle.FixedSingle;
			Font = new Font(Font.Name, 10);
			AutoSize = true;
		}
	}
}
