using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SIL.Pa.DataSource.FieldWorks;
using SIL.Pa.Model;
using SIL.Pa.Properties;
using SilTools;

namespace SIL.Pa.UI.Dialogs
{
	#region FwDataSourcePropertiesDlg class
	/// ----------------------------------------------------------------------------------------
	public partial class Fw7DataSourcePropertiesDlg : OKCancelDlgBase
	{
		private const string kFieldCol = "field";
		private const string kWsNameCol = "wsname";
		private const string kWsTypeCol = "wstype";

		private readonly PaProject m_project;
		private readonly Fw7DataSourceInfo m_dsInfo;
		private readonly List<FwWritingSysInfo> m_wsInfo;
		private readonly List<string> m_allWsNames;

		#region Construction and initialization
		/// ------------------------------------------------------------------------------------
		public Fw7DataSourcePropertiesDlg()
		{
			InitializeComponent();
		}

		/// ------------------------------------------------------------------------------------
		public Fw7DataSourcePropertiesDlg(PaProject project, Fw7DataSourceInfo fwSourceInfo,
			IEnumerable<FwWritingSysInfo> wsInfo) : this()
		{
			m_project = project;
			m_dsInfo = fwSourceInfo;

			lblProjectValue.Text = m_dsInfo.ToString();

			lblProject.Font = FontHelper.UIFont;
			lblProjectValue.Font = FontHelper.UIFont;
			grpWritingSystems.Font = FontHelper.UIFont;
			grpPhoneticDataStoreType.Font = FontHelper.UIFont;
			rbLexForm.Font = FontHelper.UIFont;
			rbPronunField.Font = FontHelper.UIFont;

			m_wsInfo = wsInfo.ToList();

			// Add a (none) option.
			m_wsInfo.Insert(0, new FwWritingSysInfo(FwDBUtils.FwWritingSystemType.None, 0, NoWritingSystemText));

			// Keep a list of all the writing system names, whether vernacular or analysis.
			m_allWsNames = m_wsInfo.Select(ws => ws.WsName).ToList();

			BuildGrid();
			LoadGrid();

			rbLexForm.Checked = 
				(m_dsInfo.PhoneticStorageMethod == FwDBUtils.PhoneticStorageMethod.LexemeForm);

			rbPronunField.Checked =
				(m_dsInfo.PhoneticStorageMethod == FwDBUtils.PhoneticStorageMethod.PronunciationField);

			m_dirty = false;
		}

		/// ------------------------------------------------------------------------------------
		private void BuildGrid()
		{
			m_grid.Name = Name + "Grid";
			m_grid.AutoGenerateColumns = false;
			m_grid.Font = FontHelper.UIFont;
			m_grid.EditingControlShowing += m_grid_EditingControlShowing;

			// Add a column for the field name.
			DataGridViewColumn col = SilGrid.CreateTextBoxColumn(kFieldCol);
			col.ReadOnly = true;
			col.Width = 150;
			m_grid.Columns.Add(col);
			App.LocalizeObject(m_grid.Columns[kFieldCol],
				"Fw7DataSourcePropertiesDlg.FieldNameColumnHdg", "Field",
				"Heading for the field column of the writing system grid on FieldWorks data source properties dialog.",
				App.kLocalizationGroupDialogs);

			// Add a column for the writing system name (or 'none').
			col = SilGrid.CreateDropDownListComboBoxColumn(kWsNameCol, m_allWsNames);
			col.Width = 110;
			m_grid.Columns.Add(col);
			App.LocalizeObject(m_grid.Columns[kWsNameCol],
				"Fw7DataSourcePropertiesDlg.WritingSystemColumnHdg", "Writing System",
				"Heading for the writing system column of the writing system grid on the FieldWorks data source properties dialog.",
				App.kLocalizationGroupDialogs);

			// Add a hidden column to store the writing system
			// type (i.e. vernacular or analysis) for the field.
			col = SilGrid.CreateTextBoxColumn(kWsTypeCol);
			col.Visible = false;
			m_grid.Columns.Add(col);

			m_grid.AutoResizeColumnHeadersHeight();

			if (Settings.Default.FwDataSourcePropertiesDlgGrid != null)
				Settings.Default.FwDataSourcePropertiesDlgGrid.InitializeGrid(m_grid);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Load up the grid with PA field names and the FW writing systems assigned to them,
		/// if any.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadGrid()
		{
			m_grid.Rows.Clear();

			// Go through each PA field and find the fields marked as FW fields that
			// are marked with either the vernacular or analysis writing system.
			foreach (var fieldInfo in m_project.FieldInfo.SortedList
				.Where(fi => fi.FwWritingSystemType != FwDBUtils.FwWritingSystemType.None))
			{
				// Add the field to the list and, for now, assume
				// it has no writing system assigned.
				int index = m_grid.Rows.Add(new object[] { fieldInfo.DisplayText,
					NoWritingSystemText, fieldInfo.FwWritingSystemType });

				// Get the row just added. Each row's tag property references an object
				// indicating what writing system is currently assigned to the row's field.
				// For now, assign all new rows no writing system (i.e. 'none').
				m_grid.Rows[index].Tag = new FwDataSourceWsInfo(fieldInfo.FwQueryFieldName, 0);

				//if (m_dsInfo.WritingSystemInfo == null)
				//    continue;

				//// Now go through the list of fields that have been assigned
				//// writing systems and find the one corresponding to the row just
				//// added. Then change the row's tag property to reference that
				//// writing system.
				//foreach (var dswsi in m_dsInfo.WritingSystemInfo.Where(dswsi => fieldInfo.FwQueryFieldName == dswsi.FieldName))
				//{
				//    m_grid.Rows[index].Tag = dswsi.Clone();

				//    foreach (FwWritingSysInfo wsi in m_wsInfo.Where(wsi => wsi.WsNumber == dswsi.Ws))
				//        m_grid.Rows[index].Cells[kWsNameCol].Value = wsi.WsName;
				//}
			}
		}

		/// ------------------------------------------------------------------------------------
		private string NoWritingSystemText
		{
			get
			{
				return App.LocalizeString("Fw7DataSourcePropertiesDlg.NoWritingSystemSpecifiedText", "(none)",
					"Item for no writing system specified for a field in the FW data source properties dialog.",
					App.kLocalizationGroupDialogs);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// We need to intercept things here so we can modify the contents of the combo. before
		/// the user drops-down its list. When the grid's writing system column is created, the
		/// entire list of analysis and vernacular writing systems are sent to its Items
		/// collection so the grid will consider any of those writing systems as valid choices.
		/// However, since some fields in the grid can only be assigned analysis writing
		/// systems and others only vernacular, we need to modify the grid's combo's list just
		/// before it's shown to only include the proper subset of writing systems for the
		/// current row's field.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		void m_grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			var cbo = e.Control as ComboBox;
			if (cbo == null || m_grid.CurrentRow == null)
				return;

			// Get the writing system type (analysis or vernacular) for the current row.
			var wsType = (FwDBUtils.FwWritingSystemType)m_grid.CurrentRow.Cells[kWsTypeCol].Value;
			
			// Get the writing system information to which the current row is set.
			var currWsInfo = m_grid.CurrentRow.Tag as FwDataSourceWsInfo;
		
			// Clear the combo list and add the '(none)' options first.
			cbo.Items.Clear();
			cbo.Items.Add(m_wsInfo[0]);

			// Now iterate through the writing systems and only add ones to the list
			// whose type is the same as the type of writing system for the current row.
			foreach (var wsInfo in m_wsInfo)
			{
				if (wsInfo.WsType == wsType)
					cbo.Items.Add(wsInfo);

				if (currWsInfo.Ws == wsInfo.WsNumber)
					cbo.SelectedItem = wsInfo;
			}

			// This should never happend, but if, by this point, the combo's
			// selected item hasn't been set, set it to the first item in the list.
			if (cbo.SelectedIndex < 0)
				cbo.SelectedIndex = 0;

			cbo.SelectionChangeCommitted += cbo_SelectionChangeCommitted;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// After the user has selected a writing system for the field, save the info.
		/// for the selected one in the row's tag property so it can be retrieved when
		/// the dialog's settings are saved when it's closed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void cbo_SelectionChangeCommitted(object sender, EventArgs e)
		{
			var cbo = sender as ComboBox;
			if (cbo == null || cbo.SelectedItem == null || m_grid.CurrentRow == null)
				return;

			var currWsInfo = m_grid.CurrentRow.Tag as FwDataSourceWsInfo;
			var pickedWs = cbo.SelectedItem as FwWritingSysInfo;

			if (currWsInfo != null && pickedWs != null)
				currWsInfo.Ws = pickedWs.WsNumber;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This will make sure that only writing system cells gets focus since they are the
		/// only one the user may change. I hate using SendKeys, but setting CurrentCell
		/// causing a reentrant error.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_grid_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 0)
				SendKeys.Send("{TAB}");
		}

		#endregion

		#region Overridden methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not values on the dialog changed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool IsDirty
		{
			get	{return base.IsDirty || m_grid.IsDirty;}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// For some reason, this needs to be done here or we end up in the second row as
		/// a result of code in the m_grid_CellEnter event.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (m_grid.Rows.Count > 0)
				m_grid.CurrentCell = m_grid[kWsNameCol, 0];
		}

		#endregion

		#region Event Handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandlePhoneticStorageTypeCheckedChanged(object sender, EventArgs e)
		{
			m_dirty = true;
		}

		#endregion

		#region Methods for verifying changes and saving them before closing
		/// ------------------------------------------------------------------------------------
		protected override void SaveSettings()
		{
			Settings.Default.FwDataSourcePropertiesDlgGrid = GridSettings.Create(m_grid);
			base.SaveSettings();
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Verifies that a writing system has been specified for at least one field.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool Verify()
		{
			if ((from DataGridViewRow row in m_grid.Rows select row.Tag as FwDataSourceWsInfo)
				.Any(wsInfo => wsInfo != null && wsInfo.Ws > 0))
			{
				return true;
			}

			var msg = App.LocalizeString("FwDataSourcePropertiesDlg.MissingWritingSystemMsg",
				"You must specify a writing system for at least one field.",
				"Message displayed in the FieldWorks data source dialog when the user clicks OK when no field has been assigned a writing system.",
				App.kLocalizationGroupDialogs);

			Utils.MsgBox(msg);
			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool SaveChanges()
		{
			try
			{
				var wsInfoList = (from DataGridViewRow row in m_grid.Rows select row.Tag)
					.OfType<FwDataSourceWsInfo>().ToList();

				m_dsInfo.WritingSystemInfo = wsInfoList;
				m_dsInfo.PhoneticStorageMethod = (rbLexForm.Checked ?
					FwDBUtils.PhoneticStorageMethod.LexemeForm :
					FwDBUtils.PhoneticStorageMethod.PronunciationField);
			}
			catch
			{
				return false;
			}

			return true;
		}

		#endregion
	}

	#endregion
}