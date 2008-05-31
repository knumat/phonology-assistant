using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using SIL.Pa;
using SIL.Pa.Data;
using SIL.FieldWorks.Common.UIAdapters;
using XCore;
using SIL.SpeechTools.Utils;

namespace SIL.Pa.AddOn
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class PaAddOnManager : IxCoreColleague
	{
		private ITMAdapter m_adapter;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public PaAddOnManager()
		{
			PaApp.AddMediatorColleague(this);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnMainViewOpened(object args)
		{
			try
			{
				if (args is PaMainWnd)
				{
					m_adapter = PaApp.TMAdapter;

					m_adapter.AddCommandItem("CmdBackupProject", "BackupProject",
						Properties.Resources.kstidBackupMenuText, null, null, null, null,
						null, Keys.None, null, Properties.Resources.kimidBackup);

					m_adapter.AddCommandItem("CmdRestoreProject", "RestoreProject",
						Properties.Resources.kstidRestoreMenuText, null, null, null, null,
						null, Keys.None, null, null);

					TMItemProperties itemProps = new TMItemProperties();
					itemProps.CommandId = "CmdBackupProject";
					itemProps.Name = "mnuBackupProject";
					itemProps.Text = null;
					m_adapter.AddMenuItem(itemProps, "mnuFile", "mnuPlayback");
					
					itemProps = new TMItemProperties();
					itemProps.CommandId = "CmdRestoreProject";
					itemProps.Name = "mnuRestoreProject";
					itemProps.Text = null;
					m_adapter.AddMenuItem(itemProps, "mnuFile", "mnuPlayback");

					AddOnHelper.AddSeparatorBeforeMenuItem("mnuPlayback");
					AddOnHelper.Initialize();
				}
			}
			catch { }

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnBackupProject(object args)
		{
			BackupDlg.Backup();
			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnUpdateBackupProject(object args)
		{
			TMItemProperties itemProps = args as TMItemProperties;
			if (itemProps == null)
				return false;

			bool enable = (PaApp.Project != null);

			if (itemProps.Enabled != enable)
			{
				itemProps.Update = true;
				itemProps.Visible = true;
				itemProps.Enabled = enable;
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected bool OnRestoreProject(object args)
		{
			RestoreDlg.Restore();
			return true;
		}

		#region IxCoreColleague Members
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Never used.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Init(Mediator mediator, XmlNode configurationParameters)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the message target.
		/// </summary>
		/// <returns></returns>
		/// ------------------------------------------------------------------------------------
		public IxCoreColleague[] GetMessageTargets()
		{
			return (new IxCoreColleague[] {this});
		}

		#endregion
	}
}
