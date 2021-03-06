// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005-2015, SIL International.
// <copyright from='2005' to='2015' company='SIL International'>
//		Copyright (c) 2005-2015, SIL International.
//    
//		This software is distributed under the MIT License, as specified in the LICENSE.txt file.
// </copyright> 
#endregion
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SIL.Pa.Model;
using SilTools;

namespace SIL.Pa.UI.Dialogs
{
	/// ----------------------------------------------------------------------------------------
	public partial class OptionsDlg
	{
		SortOptionsTypeComboItem m_prevListType;

		/// ------------------------------------------------------------------------------------
		private void InitializeSortingTab()
		{
			// This tab isn't valid if there is no project loaded.
			if (m_project == null)
			{
				tabOptions.TabPages.Remove(tpgSorting);
				return;
			}

			lblSortInfo.Font = FontHelper.UIFont;
			lblSortFldsHdr.Font = FontHelper.UIFont;
			lblListType.Font = FontHelper.UIFont;
			grpPhoneticSortOptions.Font = FontHelper.UIFont;
			grpColSortOptions.Font = FontHelper.UIFont;
			cboListType.Font = FontHelper.UIFont;
			chkSaveManual.Font = FontHelper.UIFont;
			lblSaveManual.Font = FontHelper.UIFont;

			var item = new SortOptionsTypeComboItem(cboListType.Items[0].ToString(),
				m_project.DataCorpusVwSortOptions.Copy());

			cboListType.Items.RemoveAt(0);
			cboListType.Items.Insert(0, item);

			item = new SortOptionsTypeComboItem(cboListType.Items[1].ToString(),
				m_project.SearchVwSortOptions.Copy());
			cboListType.Items.RemoveAt(1);
			cboListType.Items.Insert(1, item);

			item = new SortOptionsTypeComboItem(cboListType.Items[2].ToString(),
				m_project.DistributionChartVwSortOptions.Copy());
			cboListType.Items.RemoveAt(2);
			cboListType.Items.Add(item);

			BuildGrid();

			cboListType.Left = lblListType.Right + 10;
			cboListType.SelectedIndex = 0;
			m_sortingGrid.IsDirty = false;
			App.SetGridSelectionColors(m_sortingGrid, false);
		}

		/// ------------------------------------------------------------------------------------
		private void BuildGrid()
		{
			// Create the column for the ascending check box.
			DataGridViewColumn col = SilGrid.CreateCheckBoxColumn("include");
			col.HeaderText = string.Empty;
			col.DividerWidth = 0;
			m_sortingGrid.Columns.Add(col);

			// Create the column for the column name.
			col = SilGrid.CreateTextBoxColumn("column");
			col.ReadOnly = true;
			m_sortingGrid.Columns.Add(col);
			App.RegisterForLocalization(m_sortingGrid.Columns["column"],
				"OptionsDlg.SortingTab.SortOrderColumnColumnHeadingText", "Column");

			// Create the column for the ascending check box.
			col = SilGrid.CreateCheckBoxColumn("direction");
			m_sortingGrid.Columns.Add(col);
			App.RegisterForLocalization(m_sortingGrid.Columns["direction"],
				"OptionsDlg.SortingTab.SortOrderDirectionColumnHeadingText", "Ascending?");
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the values on the sorting tab.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SaveSortingTabChanges()
		{
			if (!IsSortOrderTabDirty)
				return;

			if (m_prevListType != null)
				m_prevListType.SortOptions = GetSortOptionsFromTab();

			var item = cboListType.Items[0] as SortOptionsTypeComboItem;
			if (item != null)
				m_project.DataCorpusVwSortOptions = item.SortOptions;

			item = cboListType.Items[1] as SortOptionsTypeComboItem;
			if (item != null)
				m_project.SearchVwSortOptions = item.SortOptions;

			item = cboListType.Items[2] as SortOptionsTypeComboItem;
			if (item != null)
				m_project.DistributionChartVwSortOptions = item.SortOptions;

			App.MsgMediator.SendMessage("SortingOptionsChanged", m_project);
		}

		/// ------------------------------------------------------------------------------------
		private bool IsSortOrderTabDirty
		{
			get
			{
				foreach (object item in cboListType.Items)
				{
					var sotcbi = item as SortOptionsTypeComboItem;
					if (sotcbi != null && sotcbi.IsDirty)
						return true;
				}
				
				return (m_sortingGrid != null && m_sortingGrid.IsDirty);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Show the sort options for the selected word list type.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleListTypeComboSelectedIndexChanged(object sender, EventArgs e)
		{
			phoneticSortOptions.AdvancedOptionsEnabled = (cboListType.SelectedIndex > 0);
			var item = cboListType.SelectedItem as SortOptionsTypeComboItem;

			if (item == null)
				return;

			// Store the options before loading the new current item's sort options.
			if (m_prevListType != null)
				m_prevListType.SortOptions = GetSortOptionsFromTab();

			m_prevListType = item;
			var sortOptions = item.SortOptions;
			phoneticSortOptions.SortOptions = sortOptions;
			chkSaveManual.Checked = sortOptions.SaveManuallySetSortOptions;
			var sortFieldNames = LoadListFromSortOptions(sortOptions);

			grpPhoneticSortOptions.Enabled = true;

			// Now look through the list of checked items in the list on the Word List
			// tab to make sure we include those items in our list of potential sort fields.
			foreach (var field in fldSelGridWrdList.GetCheckedFields())
			{
				if (!sortFieldNames.Contains(field.Name))
				{
					m_sortingGrid.Rows.Add(new object[] { false, field, true });

					// If the field is the phonetic field then disable the phonetic sort options.
					if (field.Type == FieldType.Phonetic)
						grpPhoneticSortOptions.Enabled = false;
				}
			}

			if (m_sortingGrid.RowCount > 0)
			{
				m_sortingGrid.CurrentCell = m_sortingGrid[0, 0];
				HandleSortingGridRowEnter(null, new DataGridViewCellEventArgs(0, 0));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the selector list of the fields currently in the specified sort options.
		/// </summary>
		/// <param name="sortOptions"></param>
		/// <returns>A list of field names for those fields added to the list.</returns>
		/// ------------------------------------------------------------------------------------
		private List<string> LoadListFromSortOptions(SortOptions sortOptions)
		{
			var sortFieldNames = new List<string>();
			m_sortingGrid.Rows.Clear();

			foreach (var sf in sortOptions.SortFields.Where(sf => sf.Field != null))
			{
				m_sortingGrid.Rows.Add(new object[] {true, sf.Field, sf.Ascending });
				sortFieldNames.Add(sf.Field.Name);
			}

			return sortFieldNames;
		}

		/// ------------------------------------------------------------------------------------
		private SortOptions GetSortOptionsFromTab()
		{
			var sortOptions = phoneticSortOptions.SortOptions;
			sortOptions.SaveManuallySetSortOptions = chkSaveManual.Checked;
			sortOptions.SortFields.Clear();

			for (int i = m_sortingGrid.Rows.Count - 1; i >= 0; i--)
			{
				var row = m_sortingGrid.Rows[i];
				if ((bool)row.Cells[0].Value)
				{
					sortOptions.SetPrimarySortField(row.Cells[1].Value as PaField,
						false, (bool)row.Cells[2].Value);
				}
			}

			return sortOptions;
		}

		/// ------------------------------------------------------------------------------------
		private void HandlePhoneticSortOptionsChanged(SortOptions sortOptions)
		{
			m_dirty = true;

			var item = cboListType.SelectedItem as SortOptionsTypeComboItem;

			if (item != null)
				item.IsDirty = true;
		}

		/// ------------------------------------------------------------------------------------
		private void HandleCheckSaveManualClick(object sender, EventArgs e)
		{
			m_dirty = true;

			var item = cboListType.SelectedItem as SortOptionsTypeComboItem;

			if (item != null)
				item.IsDirty = true;
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Move a column (i.e. field) up in the list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleButtonMoveSortFieldUpClick(object sender, EventArgs e)
		{
			var currRow = m_sortingGrid.CurrentRow;
			if (currRow != null)
			{
				int i = currRow.Index;
				m_sortingGrid.Rows.Remove(currRow);
				m_sortingGrid.Rows.Insert(i - 1, currRow);
				m_sortingGrid.CurrentCell = m_sortingGrid[0, i - 1];
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Move a column (i.e. field) down in the list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleButtonMoveSortFieldDownClick(object sender, EventArgs e)
		{
			var currRow = m_sortingGrid.CurrentRow;
			if (currRow != null)
			{
				int i = currRow.Index;
				m_sortingGrid.Rows.Remove(currRow);
				m_sortingGrid.Rows.Insert(i + 1, currRow);
				m_sortingGrid.CurrentCell = m_sortingGrid[0, i + 1];
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Update the enabled state of the up and down buttons.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleSortingGridRowEnter(object sender, DataGridViewCellEventArgs e)
		{
			btnMoveSortFieldUp.Enabled = (e.RowIndex > 0);
			btnMoveSortFieldDown.Enabled = (e.RowIndex > -1 && e.RowIndex < m_sortingGrid.Rows.Count - 1);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Only enable the phonetic sort options when the phonetic column is checked.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleSortingGridCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var field = m_sortingGrid[1, e.RowIndex].Value as PaField;

			if (field.Type == FieldType.Phonetic)
			{
				m_sortingGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
				grpPhoneticSortOptions.Enabled = (bool)m_sortingGrid[0, e.RowIndex].Value;
			}
		}

		#region SortOptionsTypeComboItem class
		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Encapsulates a single item in the list type combo box on the sorting options tab page.
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		private class SortOptionsTypeComboItem
		{
			private readonly string m_text;

			/// ------------------------------------------------------------------------------------
			internal SortOptionsTypeComboItem(string text, SortOptions sortOptions)
			{
				m_text = text;
				SortOptions = sortOptions;
			}

			/// ------------------------------------------------------------------------------------
			internal SortOptions SortOptions { get; set; }

			/// --------------------------------------------------------------------------------
			public bool IsDirty { get; set; }

			/// ------------------------------------------------------------------------------------
			/// <summary>
			/// Returns the text 
			/// </summary>
			/// ------------------------------------------------------------------------------------
			public override string ToString()
			{
				return m_text;
			}
		}

		#endregion
	}
}