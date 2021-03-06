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
using System.Drawing;
using System.Text;

namespace SIL.Pa
{
	public class VowelViewer : IPACharViewerBase
	{
		public VowelViewer() : base()
		{
			m_codes = new int[][]
			{
				new int[] {0x0069, 0x0079, 0x0000, 0x0000, 0x0268, 0x0289, 0x0000, 0x0000, 0x026F, 0x0075},
				new int[] {0x0000, 0x0000, 0x026A, 0x028F, 0x0000, 0x0000, 0x0000, 0x028A, 0x0000, 0x0000},
				new int[] {0x0065, 0x00F8, 0x0000, 0x0000, 0x0258, 0x0275, 0x0000, 0x0000, 0x0264, 0x006F},
				new int[] {0x0000, 0x0000, 0x0000, 0x0000, 0x0259, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000},
				new int[] {0x025B, 0x0153, 0x0000, 0x0000, 0x025C, 0x025E, 0x0000, 0x0000, 0x028C, 0x0254},
				new int[] {0x00E6, 0x0000, 0x0000, 0x0000, 0x0250, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000},
				new int[] {0x0061, 0x0276, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0251, 0x0252}
			};
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Center the schwa and the turned a across the two middle columns.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override Rectangle AdjustRectanlge(Rectangle rc, int codepoint)
		{
			if (codepoint == 0x259 || codepoint == 0x250)
				rc.X += (rc.Width / 2);

			return rc;
		}
	}
}
