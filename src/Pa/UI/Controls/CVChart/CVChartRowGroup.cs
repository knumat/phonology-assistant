// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2010, SIL International. All Rights Reserved.
// <copyright from='2010' to='2010' company='SIL International'>
//		Copyright (c) 2010, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: CVChartRowGroup.cs
// Responsibility: Olson
// 
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SilUtils;

namespace SIL.Pa.UI.Controls
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Manages painting one or more DataGridView row headings to they look like a single
	/// heading. Part of that involves drawing the group's heading text.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class CVChartRowGroup : IDisposable
	{
		private readonly CVChartGrid m_grid;
		private readonly int m_firstRowIndex;
		private readonly int m_lastRowIndex;

		public List<DataGridViewRow> Rows { get; private set; }
		public string Text { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static CVChartRowGroup Create(string headerText, int rowCount, CVChartGrid grid)
		{
			return new CVChartRowGroup(headerText, rowCount, grid);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private CVChartRowGroup(string headerText, int rowCount, CVChartGrid grid)
		{
			m_grid = grid;
			Text = headerText;

			m_lastRowIndex = grid.Rows.Add(rowCount);
			m_firstRowIndex = m_lastRowIndex - rowCount + 1;

			Rows = (from x in grid.Rows.Cast<DataGridViewRow>()
					where x.Index >= m_firstRowIndex && x.Index <= m_lastRowIndex
					select x).ToList();

			m_grid.CellPainting += HandleCellPainting;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Dispose()
		{
			if (m_grid != null)
				m_grid.CellPainting -= HandleCellPainting;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int PreferredWidth
		{
			get
			{
				// Adding 4 accounts for the double line along the group's right edge.
				using (var g = m_grid.CreateGraphics())
					return TextRenderer.MeasureText(g, Text, FontHelper.UIFont).Width + 4;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the entire group's rectangle, regardless of whether or not some or all of 
		/// it is currently not displayed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private Rectangle GroupRectangle
		{
			get
			{
				var rc = m_grid.GetCellDisplayRectangle(-1, m_firstRowIndex, false);

				var dy = (rc.Height == Rows[0].Height ?
					rc.Y : m_grid.ColumnHeadersHeight - m_grid.VerticalScrollingOffset);

				return (m_grid == null ? Rectangle.Empty :
					new Rectangle(0, dy, m_grid.RowHeadersWidth, Rows.Sum(x => x.Height)));
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private Color CurrentGroupColor
		{
			get
			{
				int rowIndex = m_grid.CurrentCellAddress.Y;
				return (rowIndex >= m_firstRowIndex && rowIndex <= m_lastRowIndex ?
					ColorHelper.LightLightHighlight : m_grid.BackgroundColor);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.ColumnIndex >= 0 || e.RowIndex < m_firstRowIndex || e.RowIndex > m_lastRowIndex)
				return;

			e.Graphics.SetClip(new Rectangle(0, m_grid.ColumnHeadersHeight,
				m_grid.RowHeadersWidth, m_grid.ClientRectangle.Height));

			var rc = GroupRectangle;

			using (var br = new SolidBrush(m_grid.BackgroundColor))
				e.Graphics.FillRectangle(br, rc);
			
			DrawDoubleLine(e, rc);

			rc.Width -= 3;

			using (var br = new SolidBrush(CurrentGroupColor))
				e.Graphics.FillRectangle(br, rc);

			rc.Width--;

			const TextFormatFlags flags = TextFormatFlags.VerticalCenter |
				TextFormatFlags.Left | TextFormatFlags.HidePrefix |
				TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis |
				TextFormatFlags.PreserveGraphicsClipping;

			TextRenderer.DrawText(e.Graphics, Text, FontHelper.UIFont, rc,
				SystemColors.WindowText, flags);

			using (var pen = new Pen(m_grid.GridColor))
				e.Graphics.DrawLine(pen, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1);

			e.Handled = true;
			e.Graphics.ResetClip();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void DrawDoubleLine(DataGridViewCellPaintingEventArgs e, Rectangle rc)
		{
			using (var pen = new Pen(m_grid.GridColor))
			{
				e.Graphics.DrawLine(pen, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom - 1);
				e.Graphics.DrawLine(pen, rc.Right - 3, rc.Y, rc.Right - 3, rc.Bottom - 1);
			}
		}
	}
}