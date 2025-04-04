using System;
using System.Drawing;
using System.Windows.Forms;
using CnSharp.Windows.Forms.Frame;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CnSharp.Windows.Forms.Editor
{
	
	public class CodeEditor : TextEditorControl, IEditor
	{
		#region Constants and Fields

		private bool modified;

		#endregion

		//private Rectangle workingScreen;
		//protected bool interceptTabKey;
		//public bool InterceptTabKey
		//{
		//    set
		//    {
		//        interceptTabKey = value;
		//    }
		//}

		#region Constructors and Destructors

		public CodeEditor()
		{
			this.InitializeComponent();
			base.Text = string.Empty;
			base.ShowVRuler = false;
			base.ShowTabs = false;
			base.ShowSpaces = false;
			base.ShowEOLMarkers = false;
			base.ShowInvalidLines = false;
			base.ActiveTextAreaControl.TextArea.DragEnter += this.CodeEditor_DragEnter;
			base.ActiveTextAreaControl.TextArea.DragOver += this.TextArea_DragOver;
			base.ActiveTextAreaControl.TextArea.DragDrop += this.CodeEditor_DragDrop;
			base.ActiveTextAreaControl.Document.DocumentChanged += (this.Document_DocumentChanged);
			base.ActiveTextAreaControl.TextArea.KeyUp += this.TextArea_KeyUp;
		}

		#endregion

		#region Public Events

		public event EventHandler CodeChanged;

		public event DragEventHandler DoWithFileDrop;

		#endregion

		#region Public Properties

		public Caret Caret
		{
			get
			{
				return this.ActiveTextAreaControl.TextArea.Caret;
			}
		}

		public bool Modified
		{
			get
			{
				return this.modified;
			}
			set
			{
				this.modified = value;
			}
		}

		//protected override bool ProcessTabKey(bool forward)
		//{
		//    if (interceptTabKey)
		//    {
		//        if (OnProcessTabKey != null)
		//            OnProcessTabKey(this, EventArgs.Empty);
		//        return true;
		//    }
		//    return base.ProcessTabKey(forward);
		//}

		//public string Text
		//{
		//    get { return TextArea.Text; }
		//    set { TextArea.Text = value; }
		//}

		public string SelectedText
		{
			get
			{
				return base.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
			}
		}

		public string TextAfterCaret
		{
			get
			{
				int offset = this.ActiveTextAreaControl.TextArea.Caret.Offset;
				return (this.Text.Length == offset + 1) ? string.Empty : this.Text.Substring(offset);
			}
		}

		public TextArea TextArea
		{
			get
			{
				return this.ActiveTextAreaControl.TextArea;
			}
		}

		public string TextBeforeCaret
		{
			get
			{
				return string.IsNullOrEmpty(this.Text)
				       	? string.Empty
				       	: this.Text.Substring(0, this.ActiveTextAreaControl.TextArea.Caret.Offset);
			}
		}

		#endregion

		#region Public Methods

		public void AppendText(string text)
		{
			base.ActiveTextAreaControl.TextArea.InsertString(text);
		}

		public void Insert(int offSet, string text)
		{
			this.ActiveTextAreaControl.TextArea.Document.Insert(offSet, text);
		}

		public void InsertAtLineHeader(string insertingString)
		{
			this.Document.UndoStack.StartUndoGroup();
			int beginLine = this.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition.Line;
			int endLine = this.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].EndPosition.Line;
			for (int i = beginLine; i <= endLine; i++)
			{
				LineSegment line = this.ActiveTextAreaControl.TextArea.Document.GetLineSegment(i);
				this.ActiveTextAreaControl.TextArea.Document.Insert(line.Offset, insertingString);
			}
			this.Document.UndoStack.EndUndoGroup();
		}

		public void Light(string lang)
		{
			base.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(lang);
		}

		public void Remove(int offSet, int length)
		{
			this.ActiveTextAreaControl.TextArea.Document.Remove(offSet, length);
		}

		public void RemoveAtLineHeadr(string removingString)
		{
			this.Document.UndoStack.StartUndoGroup();
			int beginLine = this.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition.Line;
			int endLine = this.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].EndPosition.Line;
			for (int i = beginLine; i <= endLine; i++)
			{
				LineSegment line = this.ActiveTextAreaControl.TextArea.Document.GetLineSegment(i);
				if (this.ActiveTextAreaControl.TextArea.Document.GetText(line.Offset, removingString.Length) == removingString)
				{
					this.ActiveTextAreaControl.TextArea.Document.Remove(line.Offset, removingString.Length);
				}
			}
			this.Document.UndoStack.EndUndoGroup();
		}

		public void Replace(string text)
		{
			if (base.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected)
			{
				int index = base.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].Offset;
				int length = base.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText.Length;
				base.ActiveTextAreaControl.TextArea.Document.Remove(index, length);
				base.ActiveTextAreaControl.TextArea.Document.Insert(index, text);
				base.ActiveTextAreaControl.TextArea.Refresh();
			}
			else
			{
				base.ActiveTextAreaControl.TextArea.Text = text;
			}
		}

		#endregion

		#region Methods

		private void CodeEditor_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				//string file = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
				//AppendText(CnSharp.IO.FileIO.ReadText(file));
				if (this.DoWithFileDrop != null)
				{
					this.DoWithFileDrop(sender, e);
				}
			}
		}

		private void CodeEditor_DragEnter(object sender, DragEventArgs e)
		{
			//if (e.Data.GetDataPresent(DataFormats.FileDrop))
			//    e.Effect = DragDropEffects.Copy;
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(string)))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void Document_DocumentChanged(object sender, DocumentEventArgs e)
		{
			this.modified = true;
			if (this.CodeChanged != null)
			{
				this.CodeChanged(this, e);
			}
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// textAreaPanel
			// 
			this.textAreaPanel.Size = new Size(475, 358);
			// 
			// CodeEditor
			// 
			this.Name = "CodeEditor";
			this.Size = new Size(475, 358);
			this.ResumeLayout(false);
		}

		private void TextArea_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void TextArea_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Control && (e.KeyCode == Keys.F || e.KeyCode == Keys.H))
			{
				FindAndReplaceForm.Instance.ShowFor(this, (e.KeyCode == Keys.H));
			}
		}

		#endregion
	}
}