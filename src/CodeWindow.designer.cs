﻿namespace CnSharp.Windows.Forms.Editor
{
	partial class CodeWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.codeEditor1 = new CodeEditor();
			this.SuspendLayout();
			// 
			// codeEditor1
			// 
			this.codeEditor1.AllowDrop = true;
			this.codeEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeEditor1.IsReadOnly = false;
			this.codeEditor1.Location = new System.Drawing.Point(0, 0);
			this.codeEditor1.Modified = true;
			this.codeEditor1.Name = "codeEditor1";
			this.codeEditor1.ShowVRuler = false;
			this.codeEditor1.Size = new System.Drawing.Size(292, 273);
			this.codeEditor1.TabIndex = 0;
			this.codeEditor1.Text = "codeEditor1";
			// 
			// CodeWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.codeEditor1);
			this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "CodeWindow";
			this.Text = "CodeWindow";
			this.ResumeLayout(false);

		}

		#endregion

		private  CodeEditor codeEditor1;
	}
}