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
using System.Globalization;
using System.Windows.Forms;
using SilTools;

namespace SIL.Pa.UI.Dialogs
{
	public partial class UndefinedCharactersInClassDlg : Form
	{
		/// ------------------------------------------------------------------------------------
		public UndefinedCharactersInClassDlg()
		{
			InitializeComponent();
			lblInfo.Font = FontHelper.UIFont;
			txtChars.Font = App.PhoneticFont;

			while (true)
			{
				const TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.WordBreak;
				int height = TextRenderer.MeasureText(lblInfo.Text, lblInfo.Font,
					lblInfo.ClientSize, flags).Height;

				if (height > lblInfo.Height)
					Height++;
				else
					break;
			}
		}
		
		/// ------------------------------------------------------------------------------------
		public UndefinedCharactersInClassDlg(char[] undefinedChars) : this()
		{
			for (int i = 0; i < undefinedChars.Length; i++)
			{
				txtChars.Text += undefinedChars[i].ToString(CultureInfo.InvariantCulture);
				if (i < undefinedChars.Length - 1)
					txtChars.Text += ", ";
			}
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			App.MsgMediator.SendMessage(Name + "HandleCreated", this);
		}

		/// ------------------------------------------------------------------------------------
		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Show some temp. help until the help files are ready.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnHelp_Click(object sender, EventArgs e)
		{
			App.ShowHelpTopic(this);
		}

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Escape:
                    {
                        this.Close();
                        return true;
                    }
                case Keys.Control | Keys.Tab:
                    {
                        return true;
                    }
                case Keys.Control | Keys.Shift | Keys.Tab:
                    {
                        return true;
                    }
            }
            return base.ProcessCmdKey(ref message, keys);
        }
	}
}