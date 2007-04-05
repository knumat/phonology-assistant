using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SIL.Pa;
using SIL.SpeechTools.Utils;
using SIL.Pa.Data;
using SIL.Pa.Dialogs;

namespace SIL.Pa
{
	public partial class AddCharacterDlg : OKCancelDlgBase
	{
		#region Constants
		// Define Constants
		private const string kCodePoint = "CodePoint";
		private const string kIpaChar = "IpaChar";
		private const string kHexIPAChar = "HexIPAChar";
		private const string kLblHexValue = "Hex Value";
		private const string kName = "Name";
		private const string kDescription = "Description";
		private const string kCharType = "CharType";
		private const string kCharSubType = "CharSubType";
		private const string kIgnoreType = "IgnoreType";
		private const string kIsBaseChar = "IsBaseChar";
		private const string kCanPreceedBaseChar = "CanPreceedBaseChar";
		private const string kDisplayWDottedCircle = "DisplayWDottedCircle";
		private const string kMOA = "MOA";
		private const string kPOA = "POA";
		private const string kLblMoa = "MOA Sort Order";
		private const string kLblPoa = "POA Sort Order";
		private const string kChartColumn = "ChartColumn";
		private const string kChartGroup = "ChartGroup";
		private const string kLblChartColumn = "Column";
		private const string kLblChartGroup = "Group";
		private const string kUnknown = "Unknown";
		private const string kConsonant = "Consonant";
		private const string kVowel = "Vowel";
		private const string kPulmonic = "Pulmonic";
		private const string kNonPulmonic = "Non Pulmonic";
		private const string kOtherSymbols = "Other Symbols";

		// VOWEL Groups
		private const string kClose = "Close";
		private const string kNearClose = "Near-close";
		private const string kCloseMid = "Close-mid";
		private const string kMid = "Mid";
		private const string kOpenMid = "Open-mid";
		private const string kNearOpen = "Near-open";
		private const string kOpen = "Open";
		private const string kOther = "Other";

		// VOWEL Columns
		private const string kFrontUnrounded = "Front Unrounded";
		private const string kFrontRounded = "Front Rounded";
		private const string kNearFrontUnrounded = "Near-front Unrounded";
		private const string kNearFrontRounded = "Near-front Rounded";
		private const string kCentralUnrounded = "Central Unrounded";
		private const string kCentral = "Central";
		private const string kCentralRounded = "Central Rounded";
		private const string kNearBackRounded = "Near-back Rounded";
		private const string kBackUnrounded = "Back Unrounded";
		private const string kBackRounded = "Back Rounded";

		// CONSONANT PULMONIC Columns
		private const string kVoicelessBilabial = "Voiceless Bilabial";
		private const string kVoicedBilabial = "Voiced Bilabial";
		private const string kVoicelessLabiodental = "Voiceless Labiodental";
		private const string kVoicedLabiodental = "Voiced Labiodental";
		private const string kVoicelessDental = "Voiceless Dental";
		private const string kVoicedDental = "Voiced Dental";
		private const string kVoicelessAlveolar = "Voiceless Alveolar";
		private const string kVoicedAlveolar = "Voiced Alveolar";
		private const string kVoicelessPostalveolar = "Voiceless Postalveolar";
		private const string kVoicedPostalveolar = "Voiced Postalveolar";
		private const string kVoicelessRetroflex = "Voiceless Retroflex";
		private const string kVoicedRetroflex = "Voiced Retroflex";
		private const string kVoicelessPalatal = "Voiceless Palatal";
		private const string kVoicedPalatal = "Voiced Palatal";
		private const string kVoicelessVelar = "Voiceless Velar";
		private const string kVoicedVelar = "Voiced Velar";
		private const string kVoicelessUvular = "Voiceless Uvular";
		private const string kVoicedUvular = "Voiced Uvular";
		private const string kVoicelessPharyngeal = "Voiceless Pharyngeal";
		private const string kVoicedPharyngeal = "Voiced Pharyngeal";
		private const string kVoicelessGlottal = "Voiceless Glottal";
		private const string kVoicedGlottal = "Voiced Glottal";

		// CONSONANT PULMONIC Groups
		private const string kPlosive = "Plosive";
		private const string kNasal = "Nasal";
		private const string kTrill = "Trill";
		private const string kTapOrFlap = "Tap Or Flap";
		private const string kFricative = "Fricative";
		private const string kLateralFricative = "Lateral Fricative";
		private const string kApproximant = "Approximant";
		private const string kLateralApproximant = "Lateral Approximant";
		// NON-PULMONIC Groups
		private const string kImplosive = "Implosive";
		private const string kClick = "Click";

		// NON-PULMONIC CLICK (Ejective) Columns
		private const string kBilabial = "Bilabial";
		private const string kLabiodental = "Labiodental";
		private const string kDental = "Dental";
		private const string kLateralAlveolar = "Lateral Alveolar";
		private const string kPostalveolar = "Postalveolar";
		private const string kRetroflex = "Retroflex";
		private const string kPalatoAlveolar = "Palato Alveolar";
		private const string kVelar = "Velar";
		private const string kUvular = "Uvular";
		private const string kPharyngeal = "Pharyngeal";
		private const string kGlottal = "Glottal";
		#endregion

		#region Member variables
		private int invalidCodePoint = 31;

		// MOA & POA
		// The SortedList Key is the moa or poa and the Value is the hexIpaChar
		private SortedList<float, string> m_MOA = new SortedList<float, string>();
		private SortedList<float, string> m_POA = new SortedList<float, string>();
		private float m_original_moa;
		private float m_original_poa;

		// Chart Columns and Groups
		// The SortedList Key is the col/grp number and the Value is the col/grp name
		private string m_origChartGroup = string.Empty;
		private string m_origChartColumn = string.Empty;
		private int m_origChartColumnOtherSymbol;
		private SortedList<float, string> m_ConsChartPulmonicColumns = new SortedList<float, string>();
		private SortedList<float, string> m_ConsChartPulmonicGroups = new SortedList<float, string>();
		private SortedList<float, string> m_ConsChartNonPulmonicGroups = new SortedList<float, string>();
		private SortedList<float, string> m_ConsChartOtherSymbolGroups = new SortedList<float, string>();
		private SortedList<float, string> m_ConsChartImplosiveColumns = new SortedList<float, string>();
		private SortedList<float, string> m_ConsChartClickEjectiveColumns = new SortedList<float, string>();
		private SortedList<float, string> m_VowelChartGroups = new SortedList<float, string>();
		private SortedList<float, string> m_VowelChartColumns = new SortedList<float, string>();

		private bool m_addingChar = true;
		private IPACharInfo m_charInfo;
		private List<int> m_codePoints = new List<int>();
		private PCIEditor m_pciEditor;
		private string m_saveAFeatureDropDownName;
		private string m_saveBFeatureDropDownName;
		private ulong[] m_masks = new ulong[] {0,0};
		private ulong m_binaryMask = 0;
		#endregion

		#region Constructor
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// AddCharacterDlg Constructor.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AddCharacterDlg(PCIEditor pciEditor,  bool addingChar)
		{
			InitializeComponent();

			PCIEditor.SettingsHandler.LoadFormProperties(this);
			m_pciEditor = pciEditor;

			// Prepare things to use the same feature drop-downs that are used on the
			// main form, for the feature grid columns.
			m_saveAFeatureDropDownName = m_pciEditor.m_sddpAFeatures.SavedSettingsName;
			m_saveBFeatureDropDownName = m_pciEditor.m_sddpBFeatures.SavedSettingsName;
			m_pciEditor.m_sddpAFeatures.SavedSettingsName = Name + "AFeatureDropDown";
			m_pciEditor.m_sddpBFeatures.SavedSettingsName = Name + "BFeatureDropDown";

			m_pciEditor.m_aFeatureDropdown.Closing +=
				new ToolStripDropDownClosingEventHandler(m_aFeatureDropdown_Closing);

			m_pciEditor.m_bFeatureDropdown.Closing +=
				new ToolStripDropDownClosingEventHandler(m_bFeatureDropdown_Closing);

			lblChar.Font = FontHelper.PhoneticFont;
			cboMoa.Font = FontHelper.PhoneticFont;
			cboPoa.Font = FontHelper.PhoneticFont;

			lblChar.Top = lblCharLable.Top - ((lblChar.Height - lblCharLable.Height) / 2);

			// Load the Type combo boxes with readable strings
			foreach (string type in Enum.GetNames(typeof(IPACharacterType)))
				cboType.Items.Add(SeperateWordsWithSpace(type));
			foreach (string type in Enum.GetNames(typeof(IPACharacterSubType)))
				cboSubType.Items.Add(SeperateWordsWithSpace(type));
			foreach (string type in Enum.GetNames(typeof(IPACharIgnoreTypes)))
				cboIgnoreType.Items.Add(SeperateWordsWithSpace(type));

			cboType.SelectedIndex = 0;
			cboSubType.SelectedIndex = 0;
			cboIgnoreType.SelectedIndex = 0;

			LoadChartColumnsGroups();

			m_addingChar = addingChar;
			DataGridViewRow row;
			if (m_addingChar)
			{
				row = new DataGridViewRow();
				lblUnicodeValue.Visible = false;
				txtHexValue.Text = string.Empty;
				lblChar.Text = string.Empty;
				txtCharName.Text = string.Empty;
				txtCharDesc.Text = string.Empty;

				// Load the Moa/Poa combo boxes
				m_original_moa = -1f;
				m_original_poa = -1f;

				LoadMoaPoaComboBoxes(pciEditor.m_grid);
				CreateDirtyStateHandlers();
				return;
			}

			row = pciEditor.m_grid.CurrentRow;
			if (row.Tag is IPACharInfo)
			{
				m_binaryMask = ((IPACharInfo)row.Tag).BinaryMask;
				m_masks = new ulong[] {((IPACharInfo)row.Tag).Mask0, ((IPACharInfo)row.Tag).Mask1};
				txtBinary.Text = DataUtils.BFeatureCache.GetFeaturesText(m_binaryMask);
				txtArticulatory.Text = DataUtils.AFeatureCache.GetFeaturesText(m_masks);
			}

			// Identity
			txtHexValue.Visible = false;
			lblUnicodeValue.Text = row.Cells[kHexIPAChar].Value as string;
			lblChar.Text = row.Cells[kIpaChar].Value as string;
			txtCharName.Text = row.Cells[kName].Value as string;
			txtCharDesc.Text = row.Cells[kDescription].Value as string;

			// Types
			cboType.SelectedItem = SeperateWordsWithSpace(row.Cells[kCharType].Value as string);
			cboSubType.SelectedItem = SeperateWordsWithSpace(row.Cells[kCharSubType].Value as string);
			cboIgnoreType.SelectedItem = SeperateWordsWithSpace(row.Cells[kIgnoreType].Value as string);

			// Base Character
			chkIsBase.Checked = (bool)row.Cells[kIsBaseChar].Value;
			chkPreceedBaseChar.Checked = (bool)row.Cells[kCanPreceedBaseChar].Value;
			chkDottedCircle.Checked = (bool)row.Cells[kDisplayWDottedCircle].Value;

			// Articulation - load the Moa/Poa combo boxes
			m_original_moa = float.Parse(row.Cells[kMOA].Value.ToString());
			m_original_poa = float.Parse(row.Cells[kPOA].Value.ToString());
			LoadMoaPoaComboBoxes(pciEditor.m_grid);

			// Chart Position
			if (cboType.SelectedItem.ToString() == kConsonant)
			{
				m_origChartGroup = m_ConsChartPulmonicGroups[(int)row.Cells[kChartGroup].Value];
				if (m_origChartGroup == kOtherSymbols)
					m_origChartColumnOtherSymbol = (int)row.Cells[kChartColumn].Value;
				else
					m_origChartColumn = m_ConsChartPulmonicColumns[(int)row.Cells[kChartColumn].Value];
			}
			else if (cboType.SelectedItem.ToString() == kVowel)
			{
				m_origChartGroup = m_VowelChartGroups[(int)row.Cells[kChartGroup].Value];
				m_origChartColumn = m_VowelChartColumns[(int)row.Cells[kChartColumn].Value];
			}

			if (m_origChartGroup != string.Empty)
				cboChartGroup.SelectedItem = m_origChartGroup;
			if (m_origChartGroup == kOtherSymbols)
				cboChartColumn.SelectedIndex = m_origChartColumnOtherSymbol;
			else
			{
				if (m_origChartColumn != string.Empty)
					cboChartColumn.SelectedItem = m_origChartColumn;
			}

			CreateDirtyStateHandlers();
			m_dirty = false;
		}
		
		/// --------------------------------------------------------------------------------------------
		/// <summary>
		/// Load the Chart Column and Group sorted lists.
		/// </summary>
		/// --------------------------------------------------------------------------------------------
		void LoadChartColumnsGroups()
		{
			// Vowel Groups
			m_VowelChartGroups[0] = kClose;
			m_VowelChartGroups[1] = kNearClose;
			m_VowelChartGroups[2] = kCloseMid;
			m_VowelChartGroups[3] = kMid;
			m_VowelChartGroups[4] = kOpenMid;
			m_VowelChartGroups[5] = kNearOpen;
			m_VowelChartGroups[6] = kOpen;
			m_VowelChartGroups[7] = kOther;

			// Vowel Columns
			m_VowelChartColumns[0] = kFrontUnrounded;
			m_VowelChartColumns[1] = kFrontRounded;
			m_VowelChartColumns[2] = kNearFrontUnrounded;
			m_VowelChartColumns[3] = kNearFrontRounded;
			m_VowelChartColumns[4] = kCentralUnrounded;
			m_VowelChartColumns[5] = kCentral;
			m_VowelChartColumns[6] = kCentralRounded;
			m_VowelChartColumns[7] = kNearBackRounded;
			m_VowelChartColumns[8] = kBackUnrounded;
			m_VowelChartColumns[9] = kBackRounded;

			// CONSONANT PULMONIC Columns
			m_ConsChartPulmonicColumns[0] = kVoicelessBilabial;
			m_ConsChartPulmonicColumns[1] = kVoicedBilabial;
			m_ConsChartPulmonicColumns[2] = kVoicelessLabiodental;
			m_ConsChartPulmonicColumns[3] = kVoicedLabiodental;
			m_ConsChartPulmonicColumns[4] = kVoicelessDental;
			m_ConsChartPulmonicColumns[5] = kVoicedDental;
			m_ConsChartPulmonicColumns[6] = kVoicelessAlveolar;
			m_ConsChartPulmonicColumns[7] = kVoicedAlveolar;
			m_ConsChartPulmonicColumns[8] = kVoicelessPostalveolar;
			m_ConsChartPulmonicColumns[9] = kVoicedPostalveolar;
			m_ConsChartPulmonicColumns[10] = kVoicelessRetroflex;
			m_ConsChartPulmonicColumns[11] = kVoicedRetroflex;
			m_ConsChartPulmonicColumns[12] = kVoicelessPalatal;
			m_ConsChartPulmonicColumns[13] = kVoicedPalatal;
			m_ConsChartPulmonicColumns[14] = kVoicelessVelar;
			m_ConsChartPulmonicColumns[15] = kVoicedVelar;                      
			m_ConsChartPulmonicColumns[16] = kVoicelessUvular;
			m_ConsChartPulmonicColumns[17] = kVoicedUvular;
			m_ConsChartPulmonicColumns[18] = kVoicelessPharyngeal;
			m_ConsChartPulmonicColumns[19] = kVoicedPharyngeal;
			m_ConsChartPulmonicColumns[20] = kVoicelessGlottal;
			m_ConsChartPulmonicColumns[21] = kVoicedGlottal;

			// CONSONANT PULMONIC Groups
			m_ConsChartPulmonicGroups[0] = kPlosive;
			m_ConsChartPulmonicGroups[1] = kNasal;
			m_ConsChartPulmonicGroups[2] = kTrill;
			m_ConsChartPulmonicGroups[3] = kTapOrFlap;
			m_ConsChartPulmonicGroups[4] = kFricative;
			m_ConsChartPulmonicGroups[5] = kLateralFricative;
			m_ConsChartPulmonicGroups[6] = kApproximant;
			m_ConsChartPulmonicGroups[7] = kLateralApproximant;
			m_ConsChartPulmonicGroups[8] = kImplosive;
			m_ConsChartPulmonicGroups[9] = kClick;
			m_ConsChartPulmonicGroups[10] = kOtherSymbols;

			// NON-PULMONIC Groups
			m_ConsChartNonPulmonicGroups[8] = kImplosive;
			m_ConsChartNonPulmonicGroups[9] = kClick;

			// OTHER SYMBOLS Groups
			m_ConsChartOtherSymbolGroups[10] = kOtherSymbols;

			// OTHER SYMBOLS Columns
			//m_ConsChartOtherSymbolsColumns[7] = kAlveolarLateralFlap;
			//m_ConsChartOtherSymbolsColumns[12] = kVoicelessAlveolarPalatalFricative;
			//m_ConsChartOtherSymbolsColumns[13] = kVoicedAlveolarPalatalFricative;
			//m_ConsChartOtherSymbolsColumns[14] = kVoicelessAlveolarPalatalFricative;

			// NON-PULMONIC VOICED IMPLOSIVE Columns
			m_ConsChartImplosiveColumns[1] = kVoicedBilabial;
			m_ConsChartImplosiveColumns[3] = kVoicedLabiodental;
			m_ConsChartImplosiveColumns[5] = kVoicedDental;
			m_ConsChartImplosiveColumns[7] = kVoicedAlveolar;
			m_ConsChartImplosiveColumns[9] = kVoicedPostalveolar;
			m_ConsChartImplosiveColumns[11] = kVoicedRetroflex;
			m_ConsChartImplosiveColumns[13] = kVoicedPalatal;
			m_ConsChartImplosiveColumns[15] = kVoicedVelar;         
			m_ConsChartImplosiveColumns[17] = kVoicedUvular;
			m_ConsChartImplosiveColumns[19] = kVoicedPharyngeal;
			m_ConsChartImplosiveColumns[21] = kVoicedGlottal;

			// NON-PULMONIC CLICK (Ejective) Columns
			m_ConsChartClickEjectiveColumns[0] = kBilabial;
			m_ConsChartClickEjectiveColumns[2] = kLabiodental;
			m_ConsChartClickEjectiveColumns[4] = kDental;
			m_ConsChartClickEjectiveColumns[6] = kLateralAlveolar;
			m_ConsChartClickEjectiveColumns[8] = kPostalveolar;
			m_ConsChartClickEjectiveColumns[10] = kRetroflex;
			m_ConsChartClickEjectiveColumns[12] = kPalatoAlveolar;
			m_ConsChartClickEjectiveColumns[14] = kVelar;
			m_ConsChartClickEjectiveColumns[16] = kUvular;
			m_ConsChartClickEjectiveColumns[18] = kPharyngeal;
			m_ConsChartClickEjectiveColumns[20] = kGlottal;
		}

		/// --------------------------------------------------------------------------------------------
		/// <summary>
		/// Create DirtyStateChanged event handlers.
		/// </summary>
		/// --------------------------------------------------------------------------------------------
		void CreateDirtyStateHandlers()
		{
			txtHexValue.TextChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			txtCharName.TextChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			txtCharDesc.TextChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboType.SelectedIndexChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboSubType.SelectedIndexChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboIgnoreType.SelectedIndexChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			chkIsBase.CheckStateChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			chkPreceedBaseChar.CheckStateChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			chkDottedCircle.CheckStateChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboMoa.SelectedValueChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboPoa.SelectedValueChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboChartColumn.SelectedValueChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
			cboChartGroup.SelectedValueChanged += new EventHandler(ipaCharAdd_DirtyStateChanged);
		}

		/// --------------------------------------------------------------------------------------------
		/// <summary>
		/// The data has been changed, so set the dirty flag.
		/// </summary>
		/// --------------------------------------------------------------------------------------------
		void ipaCharAdd_DirtyStateChanged(object sender, EventArgs e)
		{
			m_dirty = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Load the MOA and POA combo boxes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadMoaPoaComboBoxes(SilGrid charGrid)
		{
			foreach (DataGridViewRow gridRow in charGrid.Rows)
			{
				// Save unique codePoint's / IPAChar's for Verification later
				int codePoint = (int)gridRow.Cells[kCodePoint].Value;
				if (!m_codePoints.Contains(codePoint))
					m_codePoints.Add(codePoint);

				if (gridRow.Cells[kCodePoint].Value != null && gridRow.Cells[kCharType].Value != null)
				{
					if ((int)gridRow.Cells[kCodePoint].Value <= invalidCodePoint ||
						(string)gridRow.Cells[kCharType].Value == kUnknown)
						continue;
				}

				if (gridRow.Cells[kIpaChar].Value != null)
				{
					// Create sorted lists of the manners and points of articulation.
					float moa = float.Parse(gridRow.Cells[kMOA].Value.ToString());
					float poa = float.Parse(gridRow.Cells[kPOA].Value.ToString());
					m_MOA[moa] = gridRow.Cells[kIpaChar].Value.ToString();
					m_POA[poa] = gridRow.Cells[kIpaChar].Value.ToString();
				}
			}

			// Load the combo boxes
			foreach (KeyValuePair<float, string> moa in m_MOA)
			{
				if (moa.Key != m_original_moa)
					cboMoa.Items.Add(moa.Value);
				else
					cboMoa.SelectedIndex = cboMoa.Items.Count - 1;
			}

			foreach (KeyValuePair<float, string> poa in m_POA)
			{
				if (poa.Key != m_original_poa)
					cboPoa.Items.Add(poa.Value);
				else
					cboPoa.SelectedIndex = cboPoa.Items.Count - 1;
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Add a space before each capital letter to make the string easier to read.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private string SeperateWordsWithSpace(string multiWord)
		{
			string newName = string.Empty;
			foreach (char letter in multiWord)
			{
				if (Char.IsUpper(letter))
					newName += ' ';
				newName += letter;
			}
			return newName.Trim();
		}
		
		#endregion

		#region Private Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Load the chart group combo box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadChartGroupItems()
		{
			cboChartGroup.Items.Clear();
			cboChartColumn.Items.Clear();

			if (cboType.SelectedItem.ToString() == kConsonant)
			{
				if (cboSubType.SelectedItem.ToString() == kPulmonic)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartPulmonicGroups)
						cboChartGroup.Items.Add(col.Value);
				}
				else if (cboSubType.SelectedItem.ToString() == kNonPulmonic)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartNonPulmonicGroups)
						cboChartGroup.Items.Add(col.Value);
				}
				else if (cboSubType.SelectedItem.ToString() == kOtherSymbols)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartOtherSymbolGroups)
						cboChartGroup.Items.Add(col.Value);
				}
				else
				{
					for (int i = 0; i < 8; i++)
						cboChartGroup.Items.Add(i);
				}
			}
			else if (cboType.SelectedItem.ToString() == kVowel)
			{
				foreach (KeyValuePair<float, string> col in m_VowelChartGroups)
					cboChartGroup.Items.Add(col.Value);
			}
			else
			{
				for (int i = 0; i < 8; i++)
					cboChartGroup.Items.Add(i);
			}

			// Select the correct cbo item
			if (cboChartGroup.Items.Contains(m_origChartGroup))
				cboChartGroup.SelectedItem = m_origChartGroup;
			else if (cboChartGroup.Items.Count > 0)
				cboChartGroup.SelectedIndex = 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Load the chart column combo box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void LoadChartColumnItems()
		{
			cboChartColumn.Items.Clear();

			if (cboType.SelectedItem.ToString() == kConsonant)
			{
				if (cboChartGroup.SelectedItem.ToString() == kClick)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartClickEjectiveColumns)
						cboChartColumn.Items.Add(col.Value);
				}
				else if (cboChartGroup.SelectedItem.ToString() == kImplosive)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartImplosiveColumns)
						cboChartColumn.Items.Add(col.Value);
				}
				else if (cboSubType.SelectedItem.ToString() == kPulmonic)
				{
					foreach (KeyValuePair<float, string> col in m_ConsChartPulmonicColumns)
						cboChartColumn.Items.Add(col.Value);
				}
				else
				{
					for (int i = 0; i < 22; i++)
						cboChartColumn.Items.Add(i);
				}
			}
			else if (cboType.SelectedItem.ToString() == kVowel)
			{
				foreach (KeyValuePair<float, string> col in m_VowelChartColumns)
					cboChartColumn.Items.Add(col.Value);
			}
			else
			{
				for (int i = 0; i < 22; i++)
					cboChartColumn.Items.Add(i);
			}

			// Select the correct cbo item
			if (cboChartGroup.SelectedItem.ToString() == kOtherSymbols)
			{
				if (m_origChartGroup == kOtherSymbols)
					cboChartColumn.SelectedIndex = m_origChartColumnOtherSymbol;
			}
			else
			{
				if (cboChartColumn.Items.Contains(m_origChartColumn))
					cboChartColumn.SelectedItem = m_origChartColumn;
				else if (cboChartColumn.Items.Count > 0)
					cboChartColumn.SelectedIndex = 0;
			}
		}
		#endregion

		#region Overrides
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			m_pciEditor.m_sddpAFeatures.SavedSettingsName = m_saveAFeatureDropDownName;
			m_pciEditor.m_sddpBFeatures.SavedSettingsName = m_saveBFeatureDropDownName;
			m_pciEditor.m_aFeatureDropdown.Closing -= m_aFeatureDropdown_Closing;
			m_pciEditor.m_bFeatureDropdown.Closing -= m_bFeatureDropdown_Closing;

			PCIEditor.SettingsHandler.SaveFormProperties(this);
			base.OnFormClosing(e);
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Return the column key.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private int GetColumnKey(SortedList<float, string> sortedList)
		{
			// Check if the selectedItem is a number
			if (cboChartColumn.SelectedItem.ToString().Length < 3)
				return (int)cboChartColumn.SelectedItem;

			foreach (KeyValuePair<float, string> column in sortedList)
			{
				if (column.Value == cboChartColumn.SelectedItem.ToString())
				{
					return (int)column.Key;
				}
			}
			return 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Return the group key.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private int GetGroupKey(SortedList<float, string> sortedList)
		{
			// Check if the selectedItem is a number
			if (cboChartGroup.SelectedItem.ToString().Length < 3)
				return (int)cboChartGroup.SelectedItem;

			foreach (KeyValuePair<float, string> column in sortedList)
			{
				if (column.Value == cboChartGroup.SelectedItem.ToString())
				{
					return (int)column.Key;
				}
			}
			return 0;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the CharInfo object created from the changes made on the dialog.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public IPACharInfo CharInfo
		{
			get { return m_charInfo; }
		}
		
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Save Changes.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool SaveChanges()
		{
			m_charInfo = new IPACharInfo();

			// Identity
			m_charInfo.HexIPAChar = txtHexValue.Text;
			m_charInfo.IPAChar = lblChar.Text;
			m_charInfo.Name = txtCharName.Text;
			m_charInfo.Description = txtCharDesc.Text;
			m_charInfo.BinaryMask = m_binaryMask;
			
			// Features
			m_charInfo.Mask0 = m_masks[0];
			m_charInfo.Mask1 = m_masks[1];

			// Types - remove the spaces in the Type strings
			m_charInfo.CharType = (IPACharacterType)Enum.Parse(
				typeof(IPACharacterType), cboType.SelectedItem.ToString().Replace(" ", ""));
			m_charInfo.CharSubType = (IPACharacterSubType)Enum.Parse(
				typeof(IPACharacterSubType), cboSubType.SelectedItem.ToString().Replace(" ", ""));
			m_charInfo.IgnoreType = (IPACharIgnoreTypes)Enum.Parse(
				typeof(IPACharIgnoreTypes), cboIgnoreType.SelectedItem.ToString().Replace(" ", ""));

			// Base Character
			m_charInfo.IsBaseChar = chkIsBase.Checked;
			m_charInfo.CanPreceedBaseChar = chkPreceedBaseChar.Checked;
			m_charInfo.DisplayWDottedCircle = chkDottedCircle.Checked;

			// Save the manner of articulation sort order value
			if (cboMoa.SelectedItem == null)
			{
				// use original value if not modified
				m_charInfo.MOArticulation = (int)m_original_moa;
			}
			else
			{
				foreach (KeyValuePair<float, string> moa in m_MOA)
				{
					if (moa.Value == cboMoa.SelectedItem.ToString())
					{
						// Make sure the user actually changed the MOA
						m_charInfo.MOArticulation =
							(int)(m_original_moa == moa.Key ? m_original_moa : moa.Key + 1f);
						break;
					}
				}
			}

			// Save the place of articulation sort order value
			if (cboPoa.SelectedItem == null)
			{
				// use original value if not modified
				m_charInfo.POArticulation = (int)m_original_poa;
			}
			else
			{
				foreach (KeyValuePair<float, string> poa in m_POA)
				{
					if (poa.Value == cboPoa.SelectedItem.ToString())
					{
						// Make sure the user actually changed the POA
						m_charInfo.POArticulation =
							(int)(m_original_poa == poa.Key ? m_original_poa : poa.Key + 1f);
						break;
					}
				}
			}

			// Save Chart Position
			string subType = cboSubType.SelectedItem.ToString();
			if (cboType.SelectedItem.ToString() == kConsonant)
			{
				if (subType == kPulmonic)
				{
					m_charInfo.ChartGroup = GetGroupKey(m_ConsChartPulmonicGroups);
					m_charInfo.ChartColumn = GetColumnKey(m_ConsChartPulmonicColumns);
				}
				else if (subType == kNonPulmonic && (string)cboChartGroup.SelectedItem == kClick)
				{
					m_charInfo.ChartGroup = GetGroupKey(m_ConsChartNonPulmonicGroups);
					m_charInfo.ChartColumn = GetColumnKey(m_ConsChartClickEjectiveColumns);
				}
				else if (subType == kNonPulmonic && (string)cboChartGroup.SelectedItem == kImplosive)
				{
					m_charInfo.ChartGroup = GetGroupKey(m_ConsChartNonPulmonicGroups);
					m_charInfo.ChartColumn = GetColumnKey(m_ConsChartImplosiveColumns);
				}
				else if (subType == kOtherSymbols)
				{
					m_charInfo.ChartGroup = GetGroupKey(m_ConsChartOtherSymbolGroups);
					m_charInfo.ChartColumn = (int)cboChartColumn.SelectedItem;
				}
			}
			else if (cboType.SelectedItem.ToString() == kVowel)
			{
				m_charInfo.ChartGroup = GetGroupKey(m_VowelChartGroups);
				m_charInfo.ChartColumn = GetColumnKey(m_VowelChartColumns);
			}
			else
			{
				// The column and group selections are numbers
				//if (cboChartColumn.SelectedItem.ToString().Length < 3)
				m_charInfo.ChartColumn = (int)cboChartColumn.SelectedItem;
				//if (cboChartGroup.SelectedItem.ToString().Length < 3)
				m_charInfo.ChartGroup = (int)cboChartGroup.SelectedItem;
			}

			return true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Return true if data is OK.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected override bool Verify()
		{
			// Check for an invalid hex number
			if (m_addingChar)
			{
				try
				{
					int.Parse(txtHexValue.Text, NumberStyles.AllowHexSpecifier);
				}
				catch
				{
					STUtils.STMsgBox(Properties.Resources.kstidInvalidUnicodeValueMsg,
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				int codePoint = int.Parse(txtHexValue.Text.Trim(), NumberStyles.HexNumber);
				if (codePoint <= invalidCodePoint)
				{
					STUtils.STMsgBox(Properties.Resources.kstidUnicodeValueTooSmallMsg,
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				// Make sure the codePoint is unique
				if (m_codePoints.Contains(codePoint))
				{
					STUtils.STMsgBox(string.Format(Properties.Resources.kstidDuplicateCharMsg,
						lblChar.Text), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
			}

			string missingFields = string.Empty;
			if (m_addingChar && txtHexValue.Text == string.Empty)
				missingFields += (lblUnicode.Text + ", ");
			if (txtCharName.Text == string.Empty)
				missingFields += (lblName.Text + ", ");
			if (m_addingChar && cboMoa.SelectedItem == null)
				missingFields += (kLblMoa + ", ");
			if (m_addingChar && cboPoa.SelectedItem == null)
				missingFields += (kLblPoa + ", ");
			if (cboChartColumn.SelectedItem == null)
				missingFields += (kLblChartColumn + ", ");
			if (cboChartGroup.SelectedItem == null)
				missingFields += (kLblChartGroup + ", ");

			if (missingFields != string.Empty)
			{
				missingFields = missingFields.Replace(":", string.Empty);
				missingFields = missingFields.TrimEnd(new char[] { ',', ' ' });
				missingFields = string.Format(Properties.Resources.kstidMissingFieldsMsg, missingFields);
				STUtils.STMsgBox(missingFields, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			return true;
		}

		#endregion
		
		#region Event Handlers
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clicked the OK button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Clicked the CANCEL button.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnCancel_Click(object sender, EventArgs e)
		{
			m_dirty = false;
			m_charInfo = null;
			Close();
		}


		private void txtHexValue_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((int)e.KeyChar >= (int)'a' && (int)e.KeyChar <= (int)'f')
				e.KeyChar = (char)((int)e.KeyChar & ~0x20);
			
			if (((int)e.KeyChar >= (int)'A' && (int)e.KeyChar <= (int)'F') ||
				((int)e.KeyChar >= (int)'0' && (int)e.KeyChar <= (int)'9') ||
				e.KeyChar == '\b')
			{
				return;
			}

			e.KeyChar = '\0';
			e.Handled = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Hex Value changed.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void txtHexValue_TextChanged(object sender, EventArgs e)
		{
			if (txtHexValue.Text.Trim() == string.Empty)
				return;

			int codePoint = 0;
			if (int.TryParse(txtHexValue.Text, NumberStyles.HexNumber,
				NumberFormatInfo.InvariantInfo, out codePoint) && codePoint > 31)
			{
				lblChar.Text = ((char)codePoint).ToString();
			}
			else
				lblChar.Text = string.Empty;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handle the Selected Index Changed event for the Type combo box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void cboType_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChartGroupItems();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handle the Selected Index Changed event for the SubType combo box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void cboSubType_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChartGroupItems();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Handle the Selected Index Changed event for the Chart Group combo box.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void cboChartGroup_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChartColumnItems();
		}
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnArticulatory_Click(object sender, EventArgs e)
		{
			// Use the drop-down from the main form's grid.
			m_pciEditor.m_lvAFeatures.CurrentMasks = m_masks;
			Point pt = new Point(0, hlblArticulatory.Height);
			pt = hlblArticulatory.PointToScreen(pt);
			m_pciEditor.m_aFeatureDropdown.Show(pt);
			m_pciEditor.m_lvAFeatures.Focus();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void btnBinary_Click(object sender, EventArgs e)
		{
			// Use the drop-down from the main form's grid.
			m_pciEditor.m_lvBFeatures.CurrentMasks = new ulong[] {m_binaryMask, 0};
			Point pt = new Point(0, hlblBinary.Height);
			pt = hlblBinary.PointToScreen(pt);
			m_pciEditor.m_bFeatureDropdown.Show(pt);
			m_pciEditor.m_lvBFeatures.Focus();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the articulatory feature list based on what the user chose
		/// in the articulatory feature drop-down.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_aFeatureDropdown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (m_pciEditor.m_lvAFeatures.CurrentMasks[0] == m_masks[0] &&
				m_pciEditor.m_lvAFeatures.CurrentMasks[1] == m_masks[1])
			{
				return;
			}

			m_masks = new ulong[] {m_pciEditor.m_lvAFeatures.CurrentMasks[0],
				m_pciEditor.m_lvAFeatures.CurrentMasks[1]};

			txtArticulatory.Text = DataUtils.AFeatureCache.GetFeaturesText(m_masks);
			m_dirty = true;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Updates the binary feature list based on what the user chose in the binary feature
		/// drop-down.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void m_bFeatureDropdown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (m_pciEditor.m_lvBFeatures.CurrentMasks[0] != m_binaryMask)
			{
				m_binaryMask = m_pciEditor.m_lvBFeatures.CurrentMasks[0];
				txtBinary.Text = DataUtils.BFeatureCache.GetFeaturesText(m_binaryMask);
				m_dirty = true;
			}
		}
	}
}