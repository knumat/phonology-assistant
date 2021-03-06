// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005-2015, SIL International.
// <copyright from='2005' to='2015' company='SIL International'>
//		Copyright (c) 2005-2015, SIL International.
//    
//		This software is distributed under the MIT License, as specified in the LICENSE.txt file.
// </copyright> 
#endregion
// 
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using SilTools;

// --------------------------------------------------------------------------------------------
// Contains classes for handling ambiguous sequences. These classes replace what's in the file
// AmbiguousItemInfo.cs.
// --------------------------------------------------------------------------------------------
namespace SIL.Pa.Model
{
	/// ----------------------------------------------------------------------------------------
	[XmlType("ambiguousSequences")]
	public class AmbiguousSequences : List<AmbiguousSeq>
	{
		public const string kDefaultFileName = "DefaultAmbiguousSequences.xml";
		public const string kFileName = "AmbiguousSequences.xml";

		private static char s_unusedToken;
		
		// The parse tokens list is a hash table of character tokens unique to each ambiguous
		// sequence in the list. They are used when parsing phonetic text containing
		// ambiguous sequences. Before a phonetic transcription is parsed into phones, each
		// ambiguous sequence in the transcription is replaced by a single token. Then the
		// transcription is parsed into phones, the tokens are replaced by the ambiguous
		// sequences they represent.
		private Dictionary<char, string> m_parseTokens;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Loads the default and project-specific ambiguous sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static AmbiguousSequences Load(string projectPathPrefix)
		{
			string filename = GetFileForProject(projectPathPrefix);
			var list = XmlSerializationHelper.DeserializeFromFile<AmbiguousSequences>(filename);
			return (list ?? new AmbiguousSequences());
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the project's ambiguous sequences file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static string GetFileForProject(string projectPathPrefix)
		{
			return projectPathPrefix + kFileName;
		}

		/// ------------------------------------------------------------------------------------
		public AmbiguousSequences()
		{
			Clear();
		}

		/// ------------------------------------------------------------------------------------
		public AmbiguousSequences(IEnumerable<AmbiguousSeq> list) : base(list)
		{
			m_parseTokens = null;
			s_unusedToken = '\uFFFF';
		}

		/// ------------------------------------------------------------------------------------
		public new void Clear()
		{
			base.Clear();
			m_parseTokens = null;
			s_unusedToken = '\uFFFF';
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds an ambiguous sequence to the collection, first making sure the sequence with
		/// the same Unit value is not already in the list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public new void Add(AmbiguousSeq seq)
		{
			if (seq == null || string.IsNullOrEmpty(seq.BaseChar))
				return;

			if (!ContainsSeq(seq.Literal, false))
			{
				seq.ParseToken = s_unusedToken--;
				base.Add(seq);
			}
		}

		/// ------------------------------------------------------------------------------------
		public new void AddRange(IEnumerable<AmbiguousSeq> collection)
		{
			if (collection != null)
				base.AddRange(collection);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Adds an ambiguous sequence to the collection with the specified sequence, first
		/// making sure the unit is not already found in one of the sequences in the list.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Add(string unit)
		{
			if (!string.IsNullOrEmpty(unit) && !ContainsSeq(unit, false))
				Add(new AmbiguousSeq(unit));
		}

		/// ------------------------------------------------------------------------------------
		public new void Insert(int index, AmbiguousSeq seq)
		{
			if (seq == null || string.IsNullOrEmpty(seq.BaseChar))
				return;

			if (!ContainsSeq(seq.Literal, false))
			{
				seq.ParseToken = s_unusedToken--;
				base.Insert(index, seq);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Saves the list of ambiguous sequences to a project-specific xml file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Save(string pathPrefix)
		{
			var tmpList = new AmbiguousSequences(this);

			// Before saving, make sure there are no empty or null units
			// and get rid of those sequences that were added automatically.
			for (int i = tmpList.Count - 1; i >= 0; i--)
			{
				string unit = tmpList[i].Literal;
				if (unit == null || unit.Trim().Length == 0)
					tmpList.RemoveAt(i);
			}

			XmlSerializationHelper.SerializeToFile(pathPrefix + kFileName, tmpList);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not the specified sequence is in the collection
		/// of ambiguous sequences. When "convert" is true, it means that a match will only be
		/// returned if the item is found and the AmbiguousSeq object's convert flag is true.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool ContainsSeq(string literal, bool convert)
		{
			var seq = this.FirstOrDefault(x => x.Literal == literal);
			if (seq == null)
				return false;

			return (!convert || seq.Convert);
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
			return (from ambigSeq in this
					where ambigSeq.Literal == phone
					select (ambigSeq.Convert || !convert ? ambigSeq : null)).FirstOrDefault();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of the sequence having the specified phone.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public int GetSequenceIndex(string phone)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].Literal == phone)
					return i;
			}

			return -1;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the unit of the ambiguous sequence for the specified parse token.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string GetAmbigSeqForToken(char token)
		{
			string ambigSeq = null;

			// Create the token list if it hasn't already been built.
			if (m_parseTokens == null)
				m_parseTokens = this.ToDictionary(s => s.ParseToken, s => s.Literal);

			m_parseTokens.TryGetValue(token, out ambigSeq);
			return ambigSeq;
		}

		/// ------------------------------------------------------------------------------------
		public void SortByUnitLength()
		{
			for (int i = 0; i < Count; i++)
				this[i].DisplayIndex = i;

			Sort(AmbiguousSeqComparer);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Compare method for the length of the units of two ambiguous sequences.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static int AmbiguousSeqComparer(AmbiguousSeq x, AmbiguousSeq y)
		{
			if (x == y || ((x == null || x.Literal == null) && (y == null || y.Literal == null)))
				return 0;

			if (x == null || x.Literal == null)
				return 1;

			if (y == null || y.Literal == null)
				return -1;

			// For items of the same length, this will preserve the order in
			// which the user entered the items in the Phone Inventory view.
			if (x.Literal.Length == y.Literal.Length)
				return x.DisplayIndex.CompareTo(y.DisplayIndex);

			return -(x.Literal.Length.CompareTo(y.Literal.Length));
		}
	}

	/// ----------------------------------------------------------------------------------------
	[XmlType("sequence")]
	public class AmbiguousSeq
	{
		[XmlAttribute("literal")]
		public string Literal;

		[XmlAttribute("unit")]
		public bool Convert = true;
	
		[XmlAttribute("primaryBase")]
		public string BaseChar;
		
		// This flag is only used for sequences that were added
		// from the PhoneticParser in the IPA symbols cache.
		[XmlAttribute("generated")]
		public bool IsGenerated;

		[XmlIgnore]
		internal int DisplayIndex;
	
		[XmlIgnore]
		internal char ParseToken;

		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq()
		{
		}

		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq(string unit)
		{
			Literal = unit;

			if (string.IsNullOrEmpty(unit))
				return;

			// Find the first base character starting from the end of
			// the string. Make that character the base character for the unit.
			for (int i = unit.Length - 1; i >= 0; i--)
			{
				IPASymbol charInfo = App.IPASymbolCache[unit[i]];
				if (charInfo != null && charInfo.IsBase)
				{
					BaseChar = charInfo.Literal;
					return;
				}
			}

			// If we got this far, then we didn't find a candidate for the base character.
			// In that case, see if any of the characters are not defined in the phonetic
			// character inventory. If so, then use the first one we encounter as the base.
			for (int i = unit.Length - 1; i >= 0; i--)
			{
				if (App.IPASymbolCache[unit[i]] == null)
				{
					BaseChar = unit[i].ToString();
					return;
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		public AmbiguousSeq(string unit, bool convert, bool isGenerated) : this(unit)
		{
			Convert = convert;
			IsGenerated = isGenerated;
		}

		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return Literal;
		}
	}
}
