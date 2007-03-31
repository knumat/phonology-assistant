using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using SIL.SpeechTools.Utils;

/// --------------------------------------------------------------------------------------------
/// Contains classes for handling ambiguous sequences. These classes replace what's in the file
/// AmbiguousItemInfo.cs.
/// --------------------------------------------------------------------------------------------
namespace SIL.Pa.Data
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class AmbiguousSequences : List<AmbiguousSeq>
	{
		public const string kDefaultSequenceFile = "DefaultAmbiguousSequences.xml";
		public const string kSequenceFile = "AmbiguousSequences.xml";

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Construct the file name for the project-specific sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static string BuildFileName(string projectFileName)
		{
			string filename = (projectFileName == null ? string.Empty : projectFileName);
			filename += (filename.EndsWith(".") ? string.Empty : ".") + kSequenceFile;
			return filename;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the default and project-specific ambiguous sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static AmbiguousSequences Load(string projectFileName)
		{
			// Get the default list of sequences.
			AmbiguousSequences defaultList = STUtils.DeserializeData(kDefaultSequenceFile,
				typeof(AmbiguousSequences)) as AmbiguousSequences;

			// Make sure there is a default list.
			if (defaultList == null)
				defaultList = new AmbiguousSequences();

			// Mark the default sequences before adding the project specific ones.
			foreach (AmbiguousSeq seq in defaultList)
				seq.IsDefault = true;

			string filename = BuildFileName(projectFileName);

			// Get the project-specific sequences.
			AmbiguousSequences projectList = STUtils.DeserializeData(filename,
				typeof(AmbiguousSequences)) as AmbiguousSequences;

			if (projectList != null && projectList.Count > 0)
			{
				// If there any sequences are found in the project-specific list that are
				// also found in the default list, then remove them from the default list.
				foreach (AmbiguousSeq seq in projectList)
				{
					int i = defaultList.GetSequenceIndex(seq.Unit);
					if (i >= 0)
						defaultList.RemoveAt(i);
				}

				// Now Combine the project-specific sequences with the default ones.
				defaultList.AddRange(projectList);
			}

			return (defaultList.Count == 0 ? null : defaultList);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the list of ambiguous sequences to a project-specific xml file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Save(string projectFileName)
		{
			// Before saving, make sure there are no empty or null units.
			for (int i = Count - 1; i >= 0; i--)
			{
				if (this[i].Unit == null || this[i].Unit.Trim().Length == 0)
					RemoveAt(i);
			}
			
			string filename = BuildFileName(projectFileName);
			STUtils.SerializeData(filename, this);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not the specified sequence is in the collection
		/// of ambiguous sequences. When "convert" is true, it means that a match will only be
		/// returned if the item is found and the AmbiguousSeq object's convert flag is true.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ContainsSeq(string seq, bool convert)
		{
			foreach (AmbiguousSeq ambigSeq in this)
			{
				if (ambigSeq.Unit == seq)
					return (convert ? ambigSeq.Convert : true);
			}

			return false;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the ambiguous sequence matching the specified phone. If the specified phone 
		/// is not an ambiguous sequence, then null is returned. When "convert" is true, it
		/// means that a match will only be returned if the item is found and the AmbiguousSeq
		/// object's Convert flag is true.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq GetAmbiguousSeq(string phone, bool convert)
		{
			foreach (AmbiguousSeq ambigSeq in this)
			{
				if (ambigSeq.Unit == phone)
					return (ambigSeq.Convert || !convert ? ambigSeq : null);
			}

			return null;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of the sequence having the specified phone.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int GetSequenceIndex(string phone)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Unit == phone)
					return i;
			}

			return -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the number of sequences in the list that are not default sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int NonDefaultCount
		{
			get
			{
				int i = 0;
				foreach (AmbiguousSeq seq in this)
				{
					if (!seq.IsDefault)
						i++;
				}

				return i;
			}
		}
	}

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	[XmlType("AmbiguousSequence")]
	public class AmbiguousSeq
	{
		[XmlAttribute]
		public string Unit;
		[XmlAttribute]
		public bool Convert = true;
		[XmlAttribute]
		public string BaseChar;
		[XmlIgnore]
		public bool IsDefault = false;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq()
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq(string unit)
		{
			Unit = unit;
		}
	}
}