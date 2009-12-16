using System;
using System.Windows.Forms;
using SIL.Localize.LocalizationUtils;
using SIL.Pa.Data;
using SilUtils;

namespace SIL.Pa.UI.Dialogs
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Dialog for defining ambiguous sequences.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class AmbiguousSequencesDlg : OKCancelDlgBase, IxCoreColleague
	{
		private const string kCantDeleteDefault = "AmbiguousSequencesDlg.CantDeleteDefaultAmbiguousSeqMsg";
		private const string kCantDeleteAutoGen = "AmbiguousSequencesDlg.CantDeleteAutoGenAmbiguousSeqMsg";
		private const string kBaseCharMissing = "AmbiguousSequencesDlg.BaseCharMissingMsg";
		private const string kBaseCharNotInSeq = "AmbiguousSequencesDlg.BaseCharNotInSeqMsg";
		private const string kSeqMissing = "AmbiguousSequencesDlg.MissingSequenceMsg";
		private const string kDuplicateSeq1 = "AmbiguousSequencesDlg.DuplicateSeqMsg1";
		private const string kDuplicateSeq2 = "AmbiguousSequencesDlg.DuplicateSeqMsg2";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AmbiguousSequencesDlg()
		{
			InitializeComponent();
			PaApp.SettingsHandler.LoadFormProperties(this);

			if (!PaintingHelper.CanPaintVisualStyle())
				pnlGrid.BorderStyle = BorderStyle.Fixed3D;

			BuildGrid();
			LoadGrid();
			
			m_grid.Font = FontHelper.UIFont;
			m_grid.Columns["seq"].DefaultCellStyle.Font = FontHelper.PhoneticFont;
			m_grid.Columns["seq"].CellTemplate.Style.Font = FontHelper.PhoneticFont;
			m_grid.Columns["base"].DefaultCellStyle.Font = FontHelper.PhoneticFont;
			m_grid.Columns["base"].CellTemplate.Style.Font = FontHelper.PhoneticFont;
			m_grid.Columns["cvpattern"].DefaultCellStyle.Font = FontHelper.PhoneticFont;
			m_grid.Columns["cvpattern"].CellTemplate.Style.Font = FontHelper.PhoneticFont;

			foreach (DataGridViewRow row in m_grid.Rows)
			{
				row.Cells["seq"].Style.Font = FontHelper.PhoneticFont;
				row.Cells["base"].Style.Font = FontHelper.PhoneticFont;
				row.Cells["cvpattern"].Style.Font = FontHelper.PhoneticFont;
			}

			AdjustGridRows();
			InitStrings();
			PaApp.AddMediatorColleague(this);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// These messages may not be needed if the all validations pass after additions or
		/// edits are made in this dialog box. However, calls to GetLocalizedText are made
		/// for each possible message in order to make sure the strings are added to the
		/// strings database.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void InitStrings()
		{
			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.CantDeleteAutoGenAmbiguousSeqMsg",
				"This ambiguous sequence was automatically generated based\non phonetic " +
				"transcriptions found in one or more data sources.\nAutomatically " +
				"generated ambiguous sequences may not be\ndeleted. If you do not want " +
				"Phonology Assistant to treat this\nsequence as a unit, clear the 'Treat " +
				"as one Unit?�check box.", "Message displayed when trying to delete an " +
				"automatically generated ambiguous sequence in the ambiguous sequence " +
				"dialog box.", "Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.CantDeleteDefaultAmbiguousSeqMsg",
				"Default sequences may not be deleted.", "Message displayed when trying " +
				"to delete a default ambiguous sequence in the ambiguous sequence " +
				"dialog box.", "Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.BaseCharMissingMsg",
				"You must specify a base character.", "Message displayed when trying to " +
				"save ambiguous sequences in the ambiguous sequences dialog box, when one " +
				"or more sequence does not have a base character specified.",
				"Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.BaseCharNotInSeqMsg", 
				"Your base character must be contained\nwithin its associated ambiguous sequence.",
				"Message dislpayed in ambiguous sequences dialog box.",
				"Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.MissingSequenceMsg",
				"A base character may not be specified\nuntil you have specified an ambiguous sequence.",
				"Message dislpayed in ambiguous sequences dialog box.",
				"Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.DuplicateSeqMsg1",
				"That sequence already exists.", "Message displayed in ambiguous sequences " +
				"dialog box when identical sequences exist.", "Dialog Boxes",
				LocalizationCategory.ErrorOrWarningMessage, LocalizationPriority.Medium);

			LocalizationManager.LocalizeString("AmbiguousSequencesDlg.DuplicateSeqMsg2",
				"That sequence already exists as a default sequence.", "Message displayed in " +
				"ambiguous sequences dialog box when a user-added sequence is identical to a " +
				"default sequences.", "Dialog Boxes", LocalizationCategory.ErrorOrWarningMessage,
				LocalizationPriority.Medium);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clean up.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			PaApp.RemoveMediatorColleague(this);
			base.OnFormClosed(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void BuildGrid()
		{
			m_grid.Name = Name + "AmbigGrid";
			m_grid.AutoGenerateColumns = false;
			m_grid.AllowUserToAddRows = true;
			m_grid.AllowUserToDeleteRows = true;
			m_grid.AllowUserToOrderColumns = false;
			m_grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
			m_grid.Font = FontHelper.UIFont;

			DataGridViewColumn col = SilGrid.CreateTextBoxColumn("seq");
			col.Width = 90;
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			col.DefaultCellStyle.Font = FontHelper.PhoneticFont;
			col.CellTemplate.Style.Font = FontHelper.PhoneticFont;
			col.HeaderText = LocalizationManager.LocalizeString(Name + ".AmbiguousSeqColumnHdg",
				"Sequence", "Column heading in ambiguous sequences dialog box.",
				locExtender.LocalizationGroup, LocalizationCategory.Other,
				LocalizationPriority.High);
			
			m_grid.Columns.Add(col);

			col = SilGrid.CreateCheckBoxColumn("convert");
			col.Width = 75;
			col.CellTemplate.ValueType = typeof(bool);
			col.HeaderText = LocalizationManager.LocalizeString(Name + ".AmbiguousConvertColumnHdg",
				"Treat as one unit?", "Column heading in ambiguous sequences dialog box.",
				locExtender.LocalizationGroup, LocalizationCategory.Other,
				LocalizationPriority.High);

			m_grid.Columns.Add(col);

			col = SilGrid.CreateTextBoxColumn("base");
			col.Width = 75;
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			col.DefaultCellStyle.Font = FontHelper.PhoneticFont;
			col.CellTemplate.Style.Font = FontHelper.PhoneticFont;
			col.HeaderText = LocalizationManager.LocalizeString(Name + ".AmbiguousBaseCharColumnHdg",
				"Base Character", "Column heading in ambiguous sequences dialog box.",
				locExtender.LocalizationGroup, LocalizationCategory.Other,
				LocalizationPriority.High);
			
			m_grid.Columns.Add(col);

			col = SilGrid.CreateTextBoxColumn("cvpattern");
			col.ReadOnly = true;
			col.Width = 70;
			col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			col.DefaultCellStyle.Font = FontHelper.PhoneticFont;
			col.CellTemplate.Style.Font = FontHelper.PhoneticFont;
			col.HeaderText = LocalizationManager.LocalizeString(Name + ".AmbiguousCVPatternColumnHdg",
				"CV Pattern", "Column heading in ambiguous sequences dialog box.",
				locExtender.LocalizationGroup, LocalizationCategory.Other,
				LocalizationPriority.High);

			m_grid.Columns.Add(col);

			col = SilGrid.CreateCheckBoxColumn("default");
			col.Visible = false;
			m_grid.Columns.Add(col);

			col = SilGrid.CreateCheckBoxColumn("autodefault");
			col.Visible = false;
			m_grid.Columns.Add(col);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the grid with the ambiguous items information from the project.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadGrid()
		{
			// Uncomment if we ever go back to having a default set of ambiguous sequences.
			//bool showDefaults = chkShowDefaults.Checked ||
			//    PaApp.SettingsHandler.GetBoolSettingsValue(Name, "showdefault", true);

			bool showDefaults = true;
			int prevRow = m_grid.CurrentCellAddress.Y;

			m_grid.Rows.Clear();
			AmbiguousSequences ambigSeqList = DataUtils.IPACharCache.AmbiguousSequences;

			if (ambigSeqList == null || ambigSeqList.Count == 0)
			{
				m_grid.IsDirty = false;
				return;
			}

			bool hasDefaultSequences = false;
			m_grid.Rows.Add(ambigSeqList.Count);

			for (int i = 0; i < ambigSeqList.Count; i++)
			{
				m_grid["seq", i].Value = ambigSeqList[i].Unit;
				m_grid["convert", i].Value = ambigSeqList[i].Convert;
				m_grid["base", i].Value = ambigSeqList[i].BaseChar;
				m_grid["default", i].Value = ambigSeqList[i].IsDefault;
				m_grid["autodefault", i].Value = ambigSeqList[i].IsAutoGeneratedDefault;

				if (!string.IsNullOrEmpty(ambigSeqList[i].BaseChar))
				{
					m_grid["cvpattern", i].Value =
						PaApp.PhoneCache.GetCVPattern(ambigSeqList[i].BaseChar);
				}

				if (ambigSeqList[i].IsDefault || ambigSeqList[i].IsAutoGeneratedDefault)
				{
					m_grid.Rows[i].Cells[0].ReadOnly = true;
					hasDefaultSequences = true;
					if (!showDefaults)
						m_grid.Rows[i].Visible = false;
				}
			}

			// Select a row if there isn't one currently selected.
			if (m_grid.CurrentCellAddress.Y < 0 && m_grid.RowCountLessNewRow > 0 &&
				m_grid.Rows.GetRowCount(DataGridViewElementStates.Visible) > 0)
			{
				// Check if the previous row is a valid row.
				if (prevRow < 0 || prevRow >= m_grid.RowCountLessNewRow ||
					!m_grid.Rows[prevRow].Visible)
				{
					prevRow = m_grid.FirstDisplayedScrollingRowIndex;
				}

				m_grid.CurrentCell = m_grid[0, prevRow];
			}

			PaApp.SettingsHandler.LoadGridProperties(m_grid);
			AdjustGridRows();
			m_grid.IsDirty = false;
			chkShowDefaults.Enabled = hasDefaultSequences;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adjusts the rows in the specified grid by letting the grid calculate the row
		/// heights automatically, then adds an extra amount, found in the settings file,
		/// to each row.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void AdjustGridRows()
		{
			try
			{
				// Sometimes (or maybe always) this throws an exception when
				// the first row is the only row and is the NewRowIndex row.
				m_grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			}
			catch { }

			m_grid.AutoResizeRows();

			int extraRowHeight =
				PaApp.SettingsHandler.GetIntSettingsValue(Name, "ambiggridextrarowheight", 3);

			foreach (DataGridViewRow row in m_grid.Rows)
				row.Height += extraRowHeight;
		}

		#region Overridden methods of base class
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool IsDirty
		{
			get { return m_grid.IsDirty; }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void SaveSettings()
		{
			//foreach (DataGridViewColumn col in m_grid.Columns)
			//    col.Name = "col" + col.DisplayIndex;

			PaApp.SettingsHandler.SaveGridProperties(m_grid);
			PaApp.SettingsHandler.SaveFormProperties(this);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool SaveChanges()
		{
			if (!AmbiguousSequencesChanged)
			{
				m_grid.IsDirty = false;
				return false;
			}

			AmbiguousSequences ambigSeqList = new AmbiguousSequences();

			foreach (DataGridViewRow row in m_grid.Rows)
			{
				if (row.Index != m_grid.NewRowIndex)
				{
					string phone = row.Cells["seq"].Value as string;
					string basechar = row.Cells["base"].Value as string;

					// Don't bother saving anything if there isn't
					// a phone (sequence) or base character.
					if (phone != null && phone.Trim().Length > 0 &&
						basechar != null && basechar.Trim().Length > 0)
					{
						AmbiguousSeq seq = new AmbiguousSeq(phone.Trim());
						seq.BaseChar = basechar.Trim();
						seq.Convert = (row.Cells["convert"].Value == null ?
							false : (bool)row.Cells["convert"].Value);

						seq.IsDefault = (bool)row.Cells["default"].Value;
						seq.IsAutoGeneratedDefault = (bool)row.Cells["autodefault"].Value;
						ambigSeqList.Add(seq);
					}
				}
			}

			PaApp.MsgMediator.SendMessage("BeforeAmbiguousSequencesSaved", ambigSeqList);
			ambigSeqList.Save(PaApp.Project.ProjectPathFilePrefix);
			DataUtils.IPACharCache.AmbiguousSequences = AmbiguousSequences.Load(PaApp.Project.ProjectPathFilePrefix);
			PaApp.MsgMediator.SendMessage("AfterAmbiguousSequencesSaved", ambigSeqList);
			PaApp.Project.ReloadDataSources();
			return true;
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not there are any changes to the ambiguous
		/// sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool AmbiguousSequencesChanged
		{
			get
			{
				if (DataUtils.IPACharCache.AmbiguousSequences == null)
				{
					if (m_grid.RowCountLessNewRow > 0)
						return true;
				}
				else if (DataUtils.IPACharCache.AmbiguousSequences.Count !=
					m_grid.RowCountLessNewRow)
				{
					return true;
				}

				// Go through the ambiguous sequences in the grid and check them against
				// those found in the project's list of ambiguous sequences.
				foreach (DataGridViewRow row in m_grid.Rows)
				{
					if (row.Index == m_grid.NewRowIndex)
						continue;

					string seq = row.Cells["seq"].Value as string;
					string baseChar = row.Cells["base"].Value as string;
					bool convert = (bool)row.Cells["convert"].Value;

					AmbiguousSeq ambigSeq =
						DataUtils.IPACharCache.AmbiguousSequences.GetAmbiguousSeq(seq, false);

					if (ambigSeq == null || ambigSeq.Convert != convert || ambigSeq.BaseChar != baseChar)
						return true;
				}

				return false;
			}
		}

		#region Grid event methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Make sure the new row has its height set correctly.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			AdjustGridRows();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Make sure new rows get proper default values
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void m_grid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
		{
			e.Row.Cells["seq"].Value = string.Empty;
			e.Row.Cells["convert"].Value = true;
			e.Row.Cells["default"].Value = false;
			e.Row.Cells["autodefault"].Value = false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Validate the edited base character.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (e.RowIndex == m_grid.NewRowIndex)
				return;

			if (e.ColumnIndex == 0)
				e.Cancel = ValidateSequence(e.RowIndex, e.FormattedValue as string);
			else if (e.ColumnIndex == 2)
				e.Cancel = ValidateBaseCharacter(e.RowIndex, e.FormattedValue as string);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether or not the row edit should be cancelled due to a duplicate
		/// sequence.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool ValidateSequence(int row, string newSeq)
		{
			// Make sure a unit was specified.
			//if (string.IsNullOrEmpty(newUnit))
			//    msg = Properties.Resources.kstidAmbiguousBaseCharMissingMsg;

			for (int i = 0; i < m_grid.NewRowIndex; i++)
			{
				if (i != row && m_grid[0, i].Value as string == newSeq)
				{
					bool isDefault = ((bool)m_grid["default", row].Value ||
						(bool)m_grid["autodefault", row].Value);

					string msg = (isDefault ?
						LocalizationManager.GetString(kDuplicateSeq2) :
						LocalizationManager.GetString(kDuplicateSeq1));

					Utils.MsgBox(msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return true;
				}
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Returns whether or not the row edit should be cancelled due to an invalid
		/// base character.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool ValidateBaseCharacter(int row, string newBaseChar)
		{

			if (row < 0 || row >= m_grid.RowCount)
				return false;

			string msg = null;
			string phone = m_grid["seq", row].Value as string;

			// Check if a base character has been specified.
			if (string.IsNullOrEmpty(newBaseChar))
			{
				// No base character is fine when there isn't a sequence specified.
				if (string.IsNullOrEmpty(phone))
					return false;

				// At this point, we know we have a sequence but no base character
				msg = LocalizationManager.GetString(kBaseCharMissing);
			}

			if (msg == null)
			{
				// Make sure there is an ambiguous sequence before specifying a base character.
				if (string.IsNullOrEmpty(phone))
					msg = LocalizationManager.GetString(kSeqMissing);
			}

			// Make sure the new base character is part of the ambiguous sequence.
			if (msg == null && phone != null && !phone.Contains(newBaseChar))
				msg = LocalizationManager.GetString(kBaseCharNotInSeq);

			if (msg != null)
			{
				Utils.MsgBox(msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return true;
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			if (e.ColumnIndex == 1 && e.RowIndex == m_grid.NewRowIndex)
				e.Cancel = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			// Get the ambiguous sequence.
			string phone = m_grid["seq", e.RowIndex].Value as string;
			if (phone != null)
				phone = phone.Trim();

			if (string.IsNullOrEmpty(phone))
			{
				PaApp.MsgMediator.PostMessage("RemoveAmbiguousSeqRow", e.RowIndex);
				return;
			}

			// When the base character was edited then automatically determine
			// the C or V type of the ambiguous sequence.
			if (e.ColumnIndex == 2)
			{
				string newBaseChar = m_grid["base", e.RowIndex].Value as string;
				m_grid["cvpattern", e.RowIndex].Value =
					PaApp.PhoneCache.GetCVPattern(newBaseChar);
			}
			else if (e.ColumnIndex == 0)
			{
				PhoneInfo phoneInfo = new PhoneInfo(phone);

				string prevBaseChar = m_grid["base", e.RowIndex].Value as string;
				if (prevBaseChar == null || !phone.Contains(prevBaseChar))
				{
					string newBaseChar = phoneInfo.BaseCharacter.ToString();
					m_grid["base", e.RowIndex].Value = newBaseChar;
					m_grid["cvpattern", e.RowIndex].Value =
						PaApp.PhoneCache.GetCVPattern(newBaseChar);
				}
			}

			m_grid.IsDirty = AmbiguousSequencesChanged;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This message is called when a user edits away the ambiguous transcription. It is
		/// posted in the after cell edit event because rows cannot be removed in the after
		/// cell edit event handler.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnRemoveAmbiguousSeqRow(object args)
		{
			if (args.GetType() == typeof(int))
			{
				int rowIndex = (int)args;
				if (rowIndex >= 0 && rowIndex < m_grid.RowCountLessNewRow)
				{
					m_grid.Rows.RemoveAt(rowIndex);

					while (rowIndex >= 0 && rowIndex >= m_grid.RowCount)
						rowIndex--;

					if (rowIndex >= 0 && rowIndex < m_grid.RowCountLessNewRow)
						m_grid.CurrentCell = m_grid["seq", rowIndex];
				}
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Don't allow deleting default sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			if (e.Row == null)
				return;

			string msg = null;

			if (e.Row.Cells["autodefault"].Value != null && (bool)e.Row.Cells["autodefault"].Value)
				msg = LocalizationManager.GetString(kCantDeleteAutoGen);
			else if (e.Row.Cells["default"].Value != null && (bool)e.Row.Cells["default"].Value)
				msg = LocalizationManager.GetString(kCantDeleteDefault);

			if (msg != null)
			{
				Utils.MsgBox(msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
			}
		}

		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Change the visible state of the default ambiguous sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void chkShowDefaults_CheckedChanged(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in m_grid.Rows)
			{
				if (row.Index == m_grid.NewRowIndex)
					continue;

				if ((bool)row.Cells["default"].Value || (bool)row.Cells["autodefault"].Value)
					row.Visible = chkShowDefaults.Checked;
			}

			if (chkShowDefaults.Checked)
				return;

			int currRow = m_grid.CurrentCellAddress.Y;
			if (currRow < 0 || !m_grid.Rows[currRow].Visible)
			{
				foreach (DataGridViewRow row in m_grid.Rows)
				{
					if (row.Visible)
					{
						row.Selected = true;
						return;
					}
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void HandleHelpClick(object sender, EventArgs e)
		{
			PaApp.ShowHelpTopic("hidAmbiguousSequencesDlg");
		}

		#region IxCoreColleague Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Never used in PA.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Init(Mediator mediator, System.Xml.XmlNode configurationParameters)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the message target.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IxCoreColleague[] GetMessageTargets()
		{
			return new IxCoreColleague[] {this};
		}

		#endregion
	}
}