using SilUtils.Controls;

namespace SIL.Pa.UI.Views
{
	partial class PhoneInventoryVw
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoneInventoryVw));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.splitInventory = new System.Windows.Forms.SplitContainer();
			this.pnlPhones = new SilUtils.Controls.SilPanel();
			this.gridPhones = new SilUtils.SilGrid();
			this.pgpPhoneList = new SilUtils.Controls.SilGradientPanel();
			this.splitFeatures = new System.Windows.Forms.SplitContainer();
			this.pnlAFeatures = new SilUtils.Controls.SilPanel();
			this.txtAFeatures = new System.Windows.Forms.TextBox();
			this.hlblAFeatures = new SilUtils.Controls.HeaderLabel();
			this.btnADropDownArrow = new SilUtils.Controls.XButton();
			this.pnlBFeatures = new SilUtils.Controls.SilPanel();
			this.txtBFeatures = new System.Windows.Forms.TextBox();
			this.hlblBFeatures = new SilUtils.Controls.HeaderLabel();
			this.btnBDropDownArrow = new SilUtils.Controls.XButton();
			this.splitChanges = new System.Windows.Forms.SplitContainer();
			this.label1 = new System.Windows.Forms.Label();
			this.splitOuter = new System.Windows.Forms.SplitContainer();
			this.btnApply = new System.Windows.Forms.Button();
			this.m_tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.splitInventory.Panel1.SuspendLayout();
			this.splitInventory.Panel2.SuspendLayout();
			this.splitInventory.SuspendLayout();
			this.pnlPhones.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridPhones)).BeginInit();
			this.splitFeatures.Panel1.SuspendLayout();
			this.splitFeatures.Panel2.SuspendLayout();
			this.splitFeatures.SuspendLayout();
			this.pnlAFeatures.SuspendLayout();
			this.hlblAFeatures.SuspendLayout();
			this.pnlBFeatures.SuspendLayout();
			this.hlblBFeatures.SuspendLayout();
			this.splitChanges.Panel1.SuspendLayout();
			this.splitChanges.SuspendLayout();
			this.splitOuter.Panel1.SuspendLayout();
			this.splitOuter.Panel2.SuspendLayout();
			this.splitOuter.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitInventory
			// 
			resources.ApplyResources(this.splitInventory, "splitInventory");
			this.splitInventory.Name = "splitInventory";
			// 
			// splitInventory.Panel1
			// 
			this.splitInventory.Panel1.Controls.Add(this.pnlPhones);
			resources.ApplyResources(this.splitInventory.Panel1, "splitInventory.Panel1");
			// 
			// splitInventory.Panel2
			// 
			this.splitInventory.Panel2.Controls.Add(this.splitFeatures);
			this.splitInventory.TabStop = false;
			// 
			// pnlPhones
			// 
			this.pnlPhones.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this.pnlPhones.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlPhones.ClipTextForChildControls = true;
			this.pnlPhones.ControlReceivingFocusOnMnemonic = null;
			this.pnlPhones.Controls.Add(this.gridPhones);
			this.pnlPhones.Controls.Add(this.pgpPhoneList);
			resources.ApplyResources(this.pnlPhones, "pnlPhones");
			this.pnlPhones.DoubleBuffered = true;
			this.pnlPhones.MnemonicGeneratesClick = false;
			this.pnlPhones.Name = "pnlPhones";
			this.pnlPhones.PaintExplorerBarBackground = false;
			// 
			// gridPhones
			// 
			this.gridPhones.AllowUserToAddRows = false;
			this.gridPhones.AllowUserToDeleteRows = false;
			this.gridPhones.AllowUserToOrderColumns = true;
			this.gridPhones.AllowUserToResizeRows = false;
			this.gridPhones.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			this.gridPhones.BackgroundColor = System.Drawing.SystemColors.Window;
			this.gridPhones.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridPhones.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridPhones.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gridPhones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.gridPhones, "gridPhones");
			this.gridPhones.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(174)))));
			this.gridPhones.IsDirty = false;
			this.gridPhones.MultiSelect = false;
			this.gridPhones.Name = "gridPhones";
			this.gridPhones.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.gridPhones.RowHeadersVisible = false;
			this.gridPhones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gridPhones.ShowWaterMarkWhenDirty = true;
			this.gridPhones.WaterMark = "!";
			this.gridPhones.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPhones_CellMouseLeave);
			this.gridPhones.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPhones_RowEnter);
			this.gridPhones.GetWaterMarkRect += new SilUtils.SilGrid.GetWaterMarkRectHandler(this.HandleGetWaterMarkRect);
			this.gridPhones.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPhones_CellMouseEnter);
			// 
			// pgpPhoneList
			// 
			this.pgpPhoneList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this.pgpPhoneList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pgpPhoneList.ClipTextForChildControls = true;
			this.pgpPhoneList.ControlReceivingFocusOnMnemonic = this.gridPhones;
			resources.ApplyResources(this.pgpPhoneList, "pgpPhoneList");
			this.pgpPhoneList.DoubleBuffered = false;
			this.pgpPhoneList.MakeDark = false;
			this.pgpPhoneList.MnemonicGeneratesClick = false;
			this.pgpPhoneList.Name = "pgpPhoneList";
			this.pgpPhoneList.PaintExplorerBarBackground = false;
			// 
			// splitFeatures
			// 
			resources.ApplyResources(this.splitFeatures, "splitFeatures");
			this.splitFeatures.Name = "splitFeatures";
			// 
			// splitFeatures.Panel1
			// 
			this.splitFeatures.Panel1.Controls.Add(this.pnlAFeatures);
			// 
			// splitFeatures.Panel2
			// 
			this.splitFeatures.Panel2.Controls.Add(this.pnlBFeatures);
			this.splitFeatures.TabStop = false;
			// 
			// pnlAFeatures
			// 
			this.pnlAFeatures.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this.pnlAFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlAFeatures.ClipTextForChildControls = true;
			this.pnlAFeatures.ControlReceivingFocusOnMnemonic = null;
			this.pnlAFeatures.Controls.Add(this.txtAFeatures);
			this.pnlAFeatures.Controls.Add(this.hlblAFeatures);
			resources.ApplyResources(this.pnlAFeatures, "pnlAFeatures");
			this.pnlAFeatures.DoubleBuffered = true;
			this.pnlAFeatures.MnemonicGeneratesClick = false;
			this.pnlAFeatures.Name = "pnlAFeatures";
			this.pnlAFeatures.PaintExplorerBarBackground = false;
			// 
			// txtAFeatures
			// 
			this.txtAFeatures.BackColor = System.Drawing.SystemColors.Window;
			this.txtAFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.txtAFeatures, "txtAFeatures");
			this.txtAFeatures.Name = "txtAFeatures";
			this.txtAFeatures.ReadOnly = true;
			// 
			// hlblAFeatures
			// 
			this.hlblAFeatures.ClipTextForChildControls = true;
			this.hlblAFeatures.ControlReceivingFocusOnMnemonic = null;
			this.hlblAFeatures.Controls.Add(this.btnADropDownArrow);
			resources.ApplyResources(this.hlblAFeatures, "hlblAFeatures");
			this.hlblAFeatures.MnemonicGeneratesClick = false;
			this.hlblAFeatures.Name = "hlblAFeatures";
			this.hlblAFeatures.ShowWindowBackgroudOnTopAndRightEdge = true;
			this.hlblAFeatures.MnemonicInvoked += new System.EventHandler(this.btnADropDownArrow_Click);
			this.hlblAFeatures.Click += new System.EventHandler(this.btnADropDownArrow_Click);
			// 
			// btnADropDownArrow
			// 
			resources.ApplyResources(this.btnADropDownArrow, "btnADropDownArrow");
			this.btnADropDownArrow.BackColor = System.Drawing.Color.Transparent;
			this.btnADropDownArrow.CanBeChecked = false;
			this.btnADropDownArrow.Checked = false;
			this.btnADropDownArrow.DrawEmpty = false;
			this.btnADropDownArrow.DrawLeftArrowButton = false;
			this.btnADropDownArrow.DrawRightArrowButton = false;
			this.btnADropDownArrow.Image = null;
			this.btnADropDownArrow.Name = "btnADropDownArrow";
			this.m_tooltip.SetToolTip(this.btnADropDownArrow, resources.GetString("btnADropDownArrow.ToolTip"));
			this.btnADropDownArrow.Click += new System.EventHandler(this.btnADropDownArrow_Click);
			// 
			// pnlBFeatures
			// 
			this.pnlBFeatures.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this.pnlBFeatures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlBFeatures.ClipTextForChildControls = true;
			this.pnlBFeatures.ControlReceivingFocusOnMnemonic = null;
			this.pnlBFeatures.Controls.Add(this.txtBFeatures);
			this.pnlBFeatures.Controls.Add(this.hlblBFeatures);
			resources.ApplyResources(this.pnlBFeatures, "pnlBFeatures");
			this.pnlBFeatures.DoubleBuffered = true;
			this.pnlBFeatures.MnemonicGeneratesClick = false;
			this.pnlBFeatures.Name = "pnlBFeatures";
			this.pnlBFeatures.PaintExplorerBarBackground = false;
			// 
			// txtBFeatures
			// 
			this.txtBFeatures.BackColor = System.Drawing.SystemColors.Window;
			this.txtBFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.txtBFeatures, "txtBFeatures");
			this.txtBFeatures.Name = "txtBFeatures";
			this.txtBFeatures.ReadOnly = true;
			this.txtBFeatures.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtBFeatures_MouseMove);
			// 
			// hlblBFeatures
			// 
			this.hlblBFeatures.ClipTextForChildControls = true;
			this.hlblBFeatures.ControlReceivingFocusOnMnemonic = null;
			this.hlblBFeatures.Controls.Add(this.btnBDropDownArrow);
			resources.ApplyResources(this.hlblBFeatures, "hlblBFeatures");
			this.hlblBFeatures.MnemonicGeneratesClick = false;
			this.hlblBFeatures.Name = "hlblBFeatures";
			this.hlblBFeatures.ShowWindowBackgroudOnTopAndRightEdge = true;
			this.hlblBFeatures.MnemonicInvoked += new System.EventHandler(this.btnBDropDownArrow_Click);
			this.hlblBFeatures.Click += new System.EventHandler(this.btnBDropDownArrow_Click);
			// 
			// btnBDropDownArrow
			// 
			resources.ApplyResources(this.btnBDropDownArrow, "btnBDropDownArrow");
			this.btnBDropDownArrow.BackColor = System.Drawing.Color.Transparent;
			this.btnBDropDownArrow.CanBeChecked = false;
			this.btnBDropDownArrow.Checked = false;
			this.btnBDropDownArrow.DrawEmpty = false;
			this.btnBDropDownArrow.DrawLeftArrowButton = false;
			this.btnBDropDownArrow.DrawRightArrowButton = false;
			this.btnBDropDownArrow.Image = null;
			this.btnBDropDownArrow.Name = "btnBDropDownArrow";
			this.m_tooltip.SetToolTip(this.btnBDropDownArrow, resources.GetString("btnBDropDownArrow.ToolTip"));
			this.btnBDropDownArrow.Click += new System.EventHandler(this.btnBDropDownArrow_Click);
			// 
			// splitChanges
			// 
			resources.ApplyResources(this.splitChanges, "splitChanges");
			this.splitChanges.Name = "splitChanges";
			// 
			// splitChanges.Panel1
			// 
			this.splitChanges.Panel1.Controls.Add(this.label1);
			resources.ApplyResources(this.splitChanges.Panel1, "splitChanges.Panel1");
			// 
			// splitChanges.Panel2
			// 
			resources.ApplyResources(this.splitChanges.Panel2, "splitChanges.Panel2");
			this.splitChanges.TabStop = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// splitOuter
			// 
			resources.ApplyResources(this.splitOuter, "splitOuter");
			this.splitOuter.Name = "splitOuter";
			// 
			// splitOuter.Panel1
			// 
			this.splitOuter.Panel1.Controls.Add(this.splitInventory);
			// 
			// splitOuter.Panel2
			// 
			this.splitOuter.Panel2.Controls.Add(this.splitChanges);
			this.splitOuter.TabStop = false;
			// 
			// btnApply
			// 
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.m_tooltip.SetToolTip(this.btnApply, resources.GetString("btnApply.ToolTip"));
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// PhoneInventoryVw
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.splitOuter);
			this.Name = "PhoneInventoryVw";
			this.splitInventory.Panel1.ResumeLayout(false);
			this.splitInventory.Panel2.ResumeLayout(false);
			this.splitInventory.ResumeLayout(false);
			this.pnlPhones.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridPhones)).EndInit();
			this.splitFeatures.Panel1.ResumeLayout(false);
			this.splitFeatures.Panel2.ResumeLayout(false);
			this.splitFeatures.ResumeLayout(false);
			this.pnlAFeatures.ResumeLayout(false);
			this.pnlAFeatures.PerformLayout();
			this.hlblAFeatures.ResumeLayout(false);
			this.pnlBFeatures.ResumeLayout(false);
			this.pnlBFeatures.PerformLayout();
			this.hlblBFeatures.ResumeLayout(false);
			this.splitChanges.Panel1.ResumeLayout(false);
			this.splitChanges.ResumeLayout(false);
			this.splitOuter.Panel1.ResumeLayout(false);
			this.splitOuter.Panel2.ResumeLayout(false);
			this.splitOuter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitOuter;
		private System.Windows.Forms.SplitContainer splitChanges;
		private System.Windows.Forms.SplitContainer splitInventory;
		private System.Windows.Forms.SplitContainer splitFeatures;
		private HeaderLabel hlblAFeatures;
		private HeaderLabel hlblBFeatures;
		private SilPanel pnlPhones;
		private SilPanel pnlAFeatures;
		private SilPanel pnlBFeatures;
		private XButton btnADropDownArrow;
		private XButton btnBDropDownArrow;
		private System.Windows.Forms.TextBox txtAFeatures;
		private System.Windows.Forms.TextBox txtBFeatures;
		private SilUtils.SilGrid gridPhones;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.ToolTip m_tooltip;
		private SilGradientPanel pgpPhoneList;
		private System.Windows.Forms.Label label1;
	}
}