using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CnSharp.Windows.Forms.Editor
{
	public partial class FindAndReplaceForm : Form
	{
		#region Constants and Fields

		public bool lastSearchLoopedAround;

		public bool lastSearchWasBackward;

		private const int SpaceHeight = 30;

		private readonly TextEditorSearcher searcher;

		private static FindAndReplaceForm instance;

		private TextEditorControl editor;

		private bool replaceMode = true;

		#endregion

		#region Constructors and Destructors

		public FindAndReplaceForm()
		{
			this.InitializeComponent();
			this.searcher = new TextEditorSearcher();
		}

		#endregion

		#region Public Properties

		public static FindAndReplaceForm Instance
		{
			get
			{
				if (instance == null || instance.IsDisposed)
				{
					instance = new FindAndReplaceForm();
				}
				return instance;
			}
		}

		public bool ReplaceMode
		{
			get
			{
				return this.replaceMode;
			}
			set
			{
				if (this.replaceMode != value)
				{
					this.ChangeUI();
				}
				this.replaceMode = value;
			}
		}

		#endregion

		#region Properties

		private TextEditorControl Editor
		{
			set
			{
				this.editor = value;
				this.searcher.Document = this.editor.Document;
			}
		}

		#endregion

		#region Public Methods

		public static bool IsInRange(int x, int lo, int hi)
		{
			return x >= lo && x <= hi;
		}

		public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
		{
			if (string.IsNullOrEmpty(this.txtLookFor.Text))
			{
				return null;
			}
			this.lastSearchWasBackward = searchBackward;
			this.searcher.LookFor = this.txtLookFor.Text;
			this.searcher.MatchCase = this.chkMatchCase.Checked;
			this.searcher.MatchWholeWordOnly = this.chkMatchWholeWord.Checked;

			Caret caret = this.editor.ActiveTextAreaControl.Caret;
			if (viaF3 && this.searcher.HasScanRegion
			    && !IsInRange(caret.Offset, this.searcher.BeginOffset, this.searcher.EndOffset))
			{
				// user moved outside the originally selected region
				this.searcher.ClearScanRegion();
				this.UpdateTitleBar();
			}

			int startFrom = caret.Offset - (searchBackward ? 1 : 0);
			TextRange range = this.searcher.FindNext(startFrom, searchBackward, out this.lastSearchLoopedAround);
			if (range != null)
			{
				this.SelectResult(range);
			}
			else if (messageIfNotFound != null)
			{
				MessageBox.Show(messageIfNotFound);
			}
			return range;
		}

		public void ShowFor(TextEditorControl editor, bool replaceMode)
		{
			this.Editor = editor;

			this.searcher.ClearScanRegion();
			SelectionManager sm = editor.ActiveTextAreaControl.SelectionManager;
			if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
			{
				ISelection sel = sm.SelectionCollection[0];
				if (sel.StartPosition.Line == sel.EndPosition.Line)
				{
					this.txtLookFor.Text = sm.SelectedText;
				}
				else
				{
					this.searcher.SetScanRegion(sel);
				}
			}
			else
			{
				// Get the current word that the caret is on
				Caret caret = editor.ActiveTextAreaControl.Caret;
				int start = TextUtilities.FindWordStart(editor.Document, caret.Offset);
				int endAt = TextUtilities.FindWordEnd(editor.Document, caret.Offset);
				this.txtLookFor.Text = editor.Document.GetText(start, endAt - start);
			}

			this.ReplaceMode = replaceMode;

			this.Owner = (Form)editor.TopLevelControl;
			this.Show();

			this.txtLookFor.SelectAll();
			this.txtLookFor.Focus();
		}

		#endregion

		#region Methods

		private void ChangeUI()
		{
			this.lblReplaceWith.Visible = this.txtReplaceWith.Visible = this.replaceMode;
			this.btnReplace.Visible = this.btnReplaceAll.Visible = this.replaceMode;
			foreach (Control control in this.Controls)
			{
				if (control.Visible && (control is CheckBox || control is Button))
				{
					if (this.replaceMode)
					{
						control.Top += SpaceHeight;
						this.Height += SpaceHeight;
					}
					else
					{
						control.Top -= SpaceHeight;
						this.Height -= SpaceHeight;
					}
				}
			}
		}

		private void InsertText(string text)
		{
			TextArea textArea = this.editor.ActiveTextAreaControl.TextArea;
			textArea.Document.UndoStack.StartUndoGroup();
			try
			{
				if (textArea.SelectionManager.HasSomethingSelected)
				{
					textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
					textArea.SelectionManager.RemoveSelectedText();
				}
				textArea.InsertString(text);
			}
			finally
			{
				textArea.Document.UndoStack.EndUndoGroup();
			}
		}

		private void SelectResult(TextRange range)
		{
			TextLocation p1 = this.editor.Document.OffsetToPosition(range.Offset);
			TextLocation p2 = this.editor.Document.OffsetToPosition(range.Offset + range.Length);
			this.editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
			this.editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
			// Also move the caret to the end of the selection, because when the user 
			// presses F3, the caret is where we start searching next time.
			this.editor.ActiveTextAreaControl.Caret.Position = this.editor.Document.OffsetToPosition(range.Offset + range.Length);
		}

		private void UpdateTitleBar()
		{
			string text = this.ReplaceMode ? Common.GetLocalText("far") : Common.GetLocalText("f");
			if (this.editor != null && this.editor.FileName != null)
			{
				text += " - " + Path.GetFileName(this.editor.FileName);
			}
			if (this.searcher.HasScanRegion)
			{
				text += string.Format(" ({0})", Common.GetLocalText("selectionOnly"));
			}
			this.Text = text;
		}

		private void btnFindNext_Click(object sender, EventArgs e)
		{
			this.FindNext(false, false, Common.GetLocalText("notFound"));
		}

		private void btnFindPrevious_Click(object sender, EventArgs e)
		{
			this.FindNext(false, true, Common.GetLocalText("notFound"));
		}

		private void btnReplaceAll_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txtLookFor.Text))
			{
				return;
			}
			int count = 0;
			// BUG FIX: if the replacement string contains the original search string
			// (e.g. replace "red" with "very red") we must avoid looping around and
			// replacing forever! To fix, start replacing at beginning of region (by 
			// moving the caret) and stop as soon as we loop around.
			this.editor.ActiveTextAreaControl.Caret.Position = this.editor.Document.OffsetToPosition(this.searcher.BeginOffset);

			this.editor.Document.UndoStack.StartUndoGroup();
			try
			{
				while (this.FindNext(false, false, null) != null)
				{
					if (this.lastSearchLoopedAround)
					{
						break;
					}

					// Replace
					count++;
					this.InsertText(this.txtReplaceWith.Text);
				}
			}
			finally
			{
				this.editor.Document.UndoStack.EndUndoGroup();
			}
			if (count == 0)
			{
				MessageBox.Show(Common.GetLocalText("replaceNoFound"));
			}
			else
			{
				MessageBox.Show(string.Format(Common.GetLocalText("replacedCount"), count));
				this.Close();
			}
		}

		private void btnReplace_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txtLookFor.Text))
			{
				return;
			}
			SelectionManager sm = this.editor.ActiveTextAreaControl.SelectionManager;
			if (string.Equals(sm.SelectedText, this.txtLookFor.Text, StringComparison.OrdinalIgnoreCase))
			{
				this.InsertText(this.txtReplaceWith.Text);
			}
			this.FindNext(false, this.lastSearchWasBackward, Common.GetLocalText("notFound"));
		}

		#endregion
	}

	public class TextRange : AbstractSegment
	{
		#region Constants and Fields

		private IDocument document;

		#endregion

		#region Constructors and Destructors

		public TextRange(IDocument document, int offset, int length)
		{
			this.document = document;
			this.offset = offset;
			this.length = length;
		}

		#endregion
	}

	/// <summary>This class finds occurrances of a search string in a text 
	/// editor's IDocument... it's like Find box without a GUI.</summary>
	public class TextEditorSearcher : IDisposable
	{
		#region Constants and Fields

		public bool MatchCase;

		public bool MatchWholeWordOnly;

		private IDocument document;

		private string lookFor;

		private string lookFor2; // uppercase in case-insensitive mode

		// I would have used the TextAnchor class to represent the beginning and 
		// end of the region to scan while automatically adjusting to changes in 
		// the document--but for some reason it is sealed and its constructor is 
		// internal. Instead I use a TextMarker, which is perhaps even better as 
		// it gives me the opportunity to highlight the region. Note that all the 
		// markers and coloring information is associated with the text document, 
		// not the editor control, so TextEditorSearcher doesn't need a reference 
		// to the TextEditorControl. After adding the marker to the document, we
		// must remember to remove it when it is no longer needed.
		private TextMarker region;

		#endregion

		#region Constructors and Destructors

		~TextEditorSearcher()
		{
			this.Dispose();
		}

		#endregion

		#region Public Properties

		/// <summary>Begins the start offset for searching</summary>
		public int BeginOffset
		{
			get
			{
				if (this.region != null)
				{
					return this.region.Offset;
				}
				else
				{
					return 0;
				}
			}
		}

		public IDocument Document
		{
			get
			{
				return this.document;
			}
			set
			{
				if (this.document != value)
				{
					this.ClearScanRegion();
					this.document = value;
				}
			}
		}

		/// <summary>Begins the end offset for searching</summary>
		public int EndOffset
		{
			get
			{
				if (this.region != null)
				{
					return this.region.EndOffset;
				}
				else
				{
					return this.document.TextLength;
				}
			}
		}

		public bool HasScanRegion
		{
			get
			{
				return this.region != null;
			}
		}

		public string LookFor
		{
			get
			{
				return this.lookFor;
			}
			set
			{
				this.lookFor = value;
			}
		}

		#endregion

		#region Public Methods

		public void ClearScanRegion()
		{
			if (this.region != null)
			{
				this.document.MarkerStrategy.RemoveMarker(this.region);
				this.region = null;
			}
		}

		public void Dispose()
		{
			this.ClearScanRegion();
			GC.SuppressFinalize(this);
		}

		/// <summary>Finds next instance of LookFor, according to the search rules 
		/// (MatchCase, MatchWholeWordOnly).</summary>
		/// <param name="beginAtOffset">Offset in Document at which to begin the search</param>
		/// <remarks>If there is a match at beginAtOffset precisely, it will be returned.</remarks>
		/// <returns>Region of document that matches the search string</returns>
		public TextRange FindNext(int beginAtOffset, bool searchBackward, out bool loopedAround)
		{
			loopedAround = false;

			int startAt = this.BeginOffset, endAt = this.EndOffset;
			int curOffs = InRange(beginAtOffset, startAt, endAt);

			this.lookFor2 = this.MatchCase ? this.lookFor : this.lookFor.ToUpperInvariant();

			TextRange result;
			if (searchBackward)
			{
				result = this.FindNextIn(startAt, curOffs, true);
				if (result == null)
				{
					loopedAround = true;
					result = this.FindNextIn(curOffs, endAt, true);
				}
			}
			else
			{
				result = this.FindNextIn(curOffs, endAt, false);
				if (result == null)
				{
					loopedAround = true;
					result = this.FindNextIn(startAt, curOffs, false);
				}
			}
			return result;
		}

		/// <summary>Sets the region to search. The region is updated 
		/// automatically as the document changes.</summary>
		public void SetScanRegion(ISelection sel)
		{
			this.SetScanRegion(sel.Offset, sel.Length);
		}

		/// <summary>Sets the region to search. The region is updated 
		/// automatically as the document changes.</summary>
		public void SetScanRegion(int offset, int length)
		{
			Color bkgColor = this.document.HighlightingStrategy.GetColorFor("Default").BackgroundColor;
			this.region = new TextMarker(
				offset, length, TextMarkerType.SolidBlock, HalfMix(bkgColor, Color.FromArgb(160, 160, 160)));
			this.document.MarkerStrategy.AddMarker(this.region);
		}

        #endregion

        #region Methods

        private static Color HalfMix(Color one, Color two)
        {
            return Color.FromArgb((one.A + two.A) >> 1, (one.R + two.R) >> 1, (one.G + two.G) >> 1, (one.B + two.B) >> 1);
        }

        private static int InRange(int x, int lo, int hi)
        {
            return x < lo ? lo : (x > hi ? hi : x);
        }

        private static bool IsInRange(int x, int lo, int hi)
        {
            return x >= lo && x <= hi;
        }

        private TextRange FindNextIn(int offset1, int offset2, bool searchBackward)
		{
			offset2 -= this.lookFor.Length;
			char lookForCh = this.lookFor2[0];
			if (searchBackward)
			{
				for (int offset = offset2; offset >= offset1; offset--)
				{
					bool matchFirstChar = this.MatchCase
					                      	? (lookForCh == this.document.GetCharAt(offset))
					                      	: (lookForCh == Char.ToUpperInvariant(this.document.GetCharAt(offset)));
					bool matchWord = this.MatchWholeWordOnly ? this.IsWholeWordMatch(offset) : this.IsPartWordMatch(offset);
					if (matchFirstChar && matchWord)
					{
						return new TextRange(this.document, offset, this.lookFor.Length);
					}
				}
			}
			else
			{
				for (int offset = offset1; offset <= offset2; offset++)
				{
					bool matchFirstChar = this.MatchCase
					                      	? (lookForCh == this.document.GetCharAt(offset))
					                      	: (lookForCh == Char.ToUpperInvariant(this.document.GetCharAt(offset)));
					bool matchWord = this.MatchWholeWordOnly ? this.IsWholeWordMatch(offset) : this.IsPartWordMatch(offset);
					if (matchFirstChar && matchWord)
					{
						return new TextRange(this.document, offset, this.lookFor.Length);
					}
				}
			}
			return null;
		}

		private bool IsAlphaNumeric(int offset)
		{
			char c = this.document.GetCharAt(offset);
			return Char.IsLetterOrDigit(c) || c == '_';
		}

		private bool IsPartWordMatch(int offset)
		{
			string substr = this.document.GetText(offset, this.lookFor.Length);
			if (!this.MatchCase)
			{
				substr = substr.ToUpperInvariant();
			}
			return substr == this.lookFor2;
		}

		private bool IsWholeWordMatch(int offset)
		{
			if (this.IsWordBoundary(offset) && this.IsWordBoundary(offset + this.lookFor.Length))
			{
				return this.IsPartWordMatch(offset);
			}
			return false;
		}

		private bool IsWordBoundary(int offset)
		{
			return offset <= 0 || offset >= this.document.TextLength || !this.IsAlphaNumeric(offset - 1)
			       || !this.IsAlphaNumeric(offset);
		}

		#endregion
	}
}