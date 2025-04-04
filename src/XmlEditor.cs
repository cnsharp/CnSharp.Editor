using System.IO;
using System.Text;
using System.Xml;

namespace CnSharp.Windows.Forms.Editor
{
	public class XmlEditor : CodeEditor
	{
		public XmlEditor()
		{
			base.SetHighlighting("XML");
		}

		public override string Text
		{
			set
			{
				try
				{
					var doc = new XmlDocument();
					doc.LoadXml(value);
					var sb = new StringBuilder();
					var sw = new StringWriter(sb);
					var writer = new XmlTextWriter(sw)
					             	{
					             		Formatting = Formatting.Indented, Indentation = 4, IndentChar = ' '
					             	};
					doc.WriteContentTo(writer);
					base.Text = doc.OuterXml;
				}
				catch
				{
					base.Text = value;
				}
			}
			get { return base.Text; }
		}
	}
}