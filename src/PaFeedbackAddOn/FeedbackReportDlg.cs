using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace SIL.Pa.AddOn
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class FeedbackReportDlg : Form
	{
		private Label[] m_lblRatings;
		private int m_launchCount = 0;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public FeedbackReportDlg(int launchCount)
		{
			m_launchCount = launchCount;

			InitializeComponent();
			m_lblRatings = new Label[] { lbl1, lbl2, lbl3, lbl4, lbl5 };
			PaApp.SettingsHandler.LoadFormProperties(this);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnShown(EventArgs e)
		{
			foreach (Control ctrl in pnlSurveyInner.Controls)
			{
				if (ctrl is RatingSurveyCtrl)
					((RatingSurveyCtrl)ctrl).Clear();
			}

			base.OnShown(e);
			pnlSurveyInner_Resize(null, null);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			PaApp.SettingsHandler.SaveFormProperties(this);

			if (e.CloseReason == CloseReason.None && DialogResult != DialogResult.Cancel)
			{
				if (VerifyRatings())
					Send();
				else
					e.Cancel = true;
			}

			base.OnFormClosing(e);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void pnlSurveyOuter_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(SystemColors.WindowText, 2))
			{
				e.Graphics.DrawLine(pen, 3, pnlSurveyInner.Top - 3,
					pnlSurveyInner.Right - 3, pnlSurveyInner.Top - 3);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void pnlComments_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(SystemColors.WindowText, 2))
			{
				e.Graphics.DrawLine(pen, txtComments.Left, txtComments.Top - 3,
					txtComments.Right, txtComments.Top - 3);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void pnlSurveyInner_Resize(object sender, EventArgs e)
		{
			for (int i = 0; i < 5; i++)
			{
				Point pt = ratingSurveyCtrl1.GetChoiceLocation(i);
				m_lblRatings[i].Left = pnlSurveyOuter.PointToClient(pt).X;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void Send()
		{
			try
			{
				string mailAddress = Properties.Resources.kstidFeedbackMailAddress;
				string subject = "PA Feedback Report";
				string body = BuildMessage();
				body = body.Replace(System.Environment.NewLine, "%0A");
				body = body.Replace("\"", "%22");
				body = body.Replace("&", "%26");

				System.Diagnostics.Process prs = new System.Diagnostics.Process();
				prs.StartInfo.FileName = string.Format("mailto:{0}?subject={1}&body={2}",
					mailAddress, subject, body);
				
				prs.Start();
			}
			catch { }
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private bool VerifyRatings()
		{
			foreach (Control ctrl in pnlSurveyInner.Controls)
			{
				if (ctrl is RatingSurveyCtrl && ((RatingSurveyCtrl)ctrl).Rating == 0)
				{
					string msg = Properties.Resources.kstidMissingRatingMsg;
					return (SIL.SpeechTools.Utils.STUtils.STMsgBox(msg, MessageBoxButtons.YesNo) == DialogResult.Yes);
				}
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string BuildMessage()
		{
			StringBuilder bldr = new StringBuilder();

			bldr.AppendLine("Phonology Assistant Feedback Report");
			bldr.AppendFormat("Date: {0}", DateTime.Now.ToLongDateString());
			bldr.AppendLine();
			bldr.AppendFormat("Launches: {0}", m_launchCount);
			bldr.AppendLine();
			bldr.AppendFormat("Version: {0}", Application.ProductVersion);
			bldr.AppendLine();
			bldr.AppendLine();
			bldr.AppendLine();
			bldr.AppendLine("Ratings");
			bldr.AppendLine(string.Empty.PadLeft(30, '-'));

			for (int i = pnlSurveyInner.Controls.Count - 1; i >= 0; i--)
			{
				if (pnlSurveyInner.Controls[i] is RatingSurveyCtrl)
				{
					string item = ((RatingSurveyCtrl)pnlSurveyInner.Controls[i]).Text;
					bldr.AppendFormat("({0}) - {1}",
						((RatingSurveyCtrl)pnlSurveyInner.Controls[i]).Rating, item);
					
					bldr.AppendLine();
				}
			}

			bldr.AppendLine();
			bldr.AppendLine();
			bldr.AppendLine("Comments/Suggestions");
			bldr.AppendLine(string.Empty.PadLeft(30, '-'));
			bldr.Append(txtComments.Text.Trim());

			return bldr.ToString();
		}
	}
}