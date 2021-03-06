// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005-2015, SIL International.
// <copyright from='2005' to='2015' company='SIL International'>
//		Copyright (c) 2005-2015, SIL International.
//    
//		This software is distributed under the MIT License, as specified in the LICENSE.txt file.
// </copyright> 
#endregion
// 
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using L10NSharp;
using SIL.Pa.Model;
using System;

namespace SIL.Pa.PhoneticSearching
{
	/// ----------------------------------------------------------------------------------------
	public class SearchEngine
	{
		public enum WordBoundaryCondition
		{
			NoCondition,
			BeginningOfSearchItem,
			EndOfSearchItem
		}

		public enum ZeroOrMoreCondition
		{
			NoCondition,
			InSearchItem,
			MoreThanOneInEnvBefore,
			MoreThanOneInEnvAfter,
			NotBeginningOfEnvBefore,
			NotEndOfEnvAfter,
		}

		public enum OneOrMoreCondition
		{
			NoCondition,
			InSearchItem,
			MoreThanOneInEnvBefore,
			MoreThanOneInEnvAfter,
			NotBeginningOfEnvBefore,
			NotEndOfEnvAfter,
		}

		public const string kIgnoredPhone = "\uFFFC";
		public const string kBracketingError = "BRACKETING-ERROR";
		private static SearchQuery s_currQuery = new SearchQuery();

		private static bool s_ignoreUndefinedChars = true;

		private readonly string m_envBeforeStr = string.Empty;
		private readonly string m_envAfterStr = string.Empty;
		private readonly string m_srchItemStr = string.Empty;
		private string[] m_phones;
		int m_matchIndex;

		private readonly List<SearchQueryValidationError> _errors =
			new List<SearchQueryValidationError>();

		public PatternGroup EnvBeforePatternGroup { get; private set; }
		public PatternGroup SrchItemPatternGroup { get; private set; }
		public PatternGroup EnvAfterPatternGroup { get; private set; }

		/// ------------------------------------------------------------------------------------
		static SearchEngine()
		{
			IgnoredPhones = new List<string>();
			IgnoredChars = new List<char>();
			IgnoreDiacritics = true;
		}

		/// ------------------------------------------------------------------------------------
		public SearchEngine(SearchQuery query, Dictionary<string, IPhoneInfo> phoneCache)
			: this(query)
		{
			PhoneCache = phoneCache;

			if (_errors != null && _errors.Count > 0)
				query.Errors.AddRange(_errors);
		}

		/// ------------------------------------------------------------------------------------
		public SearchEngine(SearchQuery query)
		{
			CurrentSearchQuery = query;

			_errors.Clear();

			try
			{
				var parser = new PatternParser(App.Project);
				SrchItemPatternGroup = parser.Parse(query.SearchItem, EnvironmentType.Item);
				EnvBeforePatternGroup = parser.Parse(query.PrecedingEnvironment, EnvironmentType.Before);
				EnvAfterPatternGroup = parser.Parse(query.FollowingEnvironment, EnvironmentType.After);
			}
			catch
			{
				var error = new SearchQueryValidationError(
					string.Format(GetPatternSyntaxErrorMsg(), App.kEmptyDiamondPattern));
				
				_errors.Add(error);
			}

			m_srchItemStr = query.SearchItem;
			m_envBeforeStr = query.PrecedingEnvironment;
			m_envAfterStr = query.FollowingEnvironment;

			if (_errors != null && _errors.Count > 0)
				query.Errors.AddRange(_errors);
		}

		/// ------------------------------------------------------------------------------------
		public static string GetEnvironmentTypeString(EnvironmentType type)
		{
			switch (type)
			{
				case EnvironmentType.Before: return LocalizationManager.GetString("PhoneticSearchingMessages.EnvironmentBefore", "preceding environment");
				case EnvironmentType.After: return LocalizationManager.GetString("PhoneticSearchingMessages.EnvironmentAfter", "following environment");
				case EnvironmentType.Item: return LocalizationManager.GetString("PhoneticSearchingMessages.SearchItem", "search item");
			}

			return null;
		}

		/// ------------------------------------------------------------------------------------
		public static string GetSyntaxErrorMsg()
		{
			return LocalizationManager.GetString("PhoneticSearchingMessages.SyntaxErrorMsg", "Syntax error in {0}.");
		}

		/// ------------------------------------------------------------------------------------
		private static string GetPatternSyntaxErrorMsg()
		{
			return LocalizationManager.GetString("PhoneticSearchingMessages.PatternSyntaxErrorMsg",
				"Syntax error. Pattern is not in the correct format: {0}");
		}

		#region Properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the phone cache the search engine will use when searching.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static Dictionary<string, IPhoneInfo> PhoneCache { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the search options used for subsequent searching.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static SearchQuery CurrentSearchQuery
		{
			get { return s_currQuery; }
			set
			{
				s_currQuery = value;
				IgnoreDiacritics = value.IgnoreDiacritics;

				IgnoredPhones.Clear();
				IgnoredChars.Clear();

				// Go through the ignored items and move those that are base characters
				// or complete phones (e.g. tone stick figures) to one collection and
				// those that aren't to another. It's assumed that ignored items that
				// are not base characters are only one codepoint in length.
				foreach (string ignoredItem in value.GetIgnoredCharacters())
				{
					var charInfo = App.IPASymbolCache[ignoredItem];
					if (charInfo != null)
					{
						if (charInfo.IsBase)
							IgnoredPhones.Add(ignoredItem);
						else
							IgnoredChars.Add(ignoredItem[0]);
					}
				}

				if (s_ignoreUndefinedChars)
					MergeInUndefinedIgnoredPhones();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Merges the undefined list of ignored phones with the main list of ignored phones.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void MergeInUndefinedIgnoredPhones()
		{
			if (App.IPASymbolCache.UndefinedCharacters != null && IgnoredPhones != null)
			{
				foreach (var upci in App.IPASymbolCache.UndefinedCharacters
					.Where(upci => !IgnoredPhones.Contains(upci.Character.ToString(CultureInfo.InvariantCulture))))
				{
					IgnoredPhones.Add(upci.Character.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the undefined list of ignored phones from the main list of ignored phones.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static void UnMergeInUndefinedIgnoredPhones()
		{
			if (App.IPASymbolCache.UndefinedCharacters != null && IgnoredPhones != null)
			{
				foreach (var upci in App.IPASymbolCache.UndefinedCharacters
					.Where(upci => IgnoredPhones.Contains(upci.Character.ToString(CultureInfo.InvariantCulture))))
				{
					IgnoredPhones.Remove(upci.Character.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		/// ------------------------------------------------------------------------------------
		public static List<char> IgnoredChars { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a collection of all the characters to ignore when searching.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static List<string> IgnoredPhones { get; private set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not diacritics are ignored when searching.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static bool IgnoreDiacritics { get; private set; }

		/// ------------------------------------------------------------------------------------
		public static bool IgnoreUndefinedCharacters
		{
			get { return s_ignoreUndefinedChars; }
			set
			{
				s_ignoreUndefinedChars = value;

				if (value)
					MergeInUndefinedIgnoredPhones();
				else
					UnMergeInUndefinedIgnoredPhones();
			}
		}

		///// ------------------------------------------------------------------------------------
		///// <summary>
		///// Gets a string of phones found in all the IPA character and IPA character run
		///// members of all the pattern pieces (i.e. search item and before and after
		///// environments).
		///// </summary>
		///// ------------------------------------------------------------------------------------
		//public string[] GetPhonesInPattern()
		//{
		//    var bldrPhones = new StringBuilder();
		//    bldrPhones.Append(GetPhonesFromMember(SrchItemPatternGroup));
		//    bldrPhones.Append(GetPhonesFromMember(EnvBeforePatternGroup));
		//    bldrPhones.Append(GetPhonesFromMember(EnvAfterPatternGroup));

		//    return (bldrPhones.Length == 0 ? null :
		//        App.Project.PhoneticParser.Parse(bldrPhones.ToString(), true, false));
		//}

		///// ------------------------------------------------------------------------------------
		//public string[] GetPhonesInSearchItem()
		//{
		//    var bldrPhones = new StringBuilder(GetPhonesFromMember(SrchItemPatternGroup));
		//    return (bldrPhones.Length == 0 ? null :
		//        App.Project.PhoneticParser.Parse(bldrPhones.ToString(), true,
		//        ConvertPatternWithTranscriptionChanges));
		//}

		///// ------------------------------------------------------------------------------------
		//public string[] GetPhonesInPrecedingEnv()
		//{
		//    var bldrPhones = new StringBuilder(GetPhonesFromMember(EnvBeforePatternGroup));
		//    return (bldrPhones.Length == 0 ? null :
		//        App.Project.PhoneticParser.Parse(bldrPhones.ToString(), true,
		//        ConvertPatternWithTranscriptionChanges));
		//}

		/// ------------------------------------------------------------------------------------
		//public string[] GetPhonesInFollowingEnv()
		//{
		//    var bldrPhones = new StringBuilder(GetPhonesFromMember(EnvAfterPatternGroup));
		//    return (bldrPhones.Length == 0 ? null :
		//        App.Project.PhoneticParser.Parse(bldrPhones.ToString(), true,
		//        ConvertPatternWithTranscriptionChanges));
		//}
		
		///// ------------------------------------------------------------------------------------
		///// <summary>
		///// Gets an array of undefined phonetic characters found in all the IPA character and
		///// IPA character run members of all the pattern pieces (i.e. search item and before
		///// and after environments). 
		///// </summary>
		///// ------------------------------------------------------------------------------------
		//public char[] GetInvalidSymbolsInPattern()
		//{
		//    var undefinedChars = new List<char>();
		//    undefinedChars.AddRange(GetInvalidSymbolsInSearchItem());
		//    undefinedChars.AddRange(GetInvalidSymbolsInPrecedingEnv());
		//    undefinedChars.AddRange(GetInvalidSymbolsInFollowingEnv());
		//    return (undefinedChars.Count == 0 ? null : undefinedChars.ToArray());
		//}

		///// ------------------------------------------------------------------------------------
		//public List<char> GetInvalidSymbolsInSearchItem()
		//{
		//    return GetInvalidCharsFromMember(SrchItemPatternGroup);
		//}

		///// ------------------------------------------------------------------------------------
		//public List<char> GetInvalidSymbolsInPrecedingEnv()
		//{
		//    return GetInvalidCharsFromMember(EnvBeforePatternGroup);
		//}

		///// ------------------------------------------------------------------------------------
		//public List<char> GetInvalidSymbolsInFollowingEnv()
		//{
		//    return GetInvalidCharsFromMember(EnvAfterPatternGroup);
		//}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not there is an invalid word boundary symbol
		/// found in the search pattern, and where it is.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public WordBoundaryCondition GetWordBoundaryCondition()
		{
			string srchItemPattern = (SrchItemPatternGroup == null ? string.Empty : SrchItemPatternGroup.ToString());
            if (srchItemPattern.StartsWith("#", StringComparison.Ordinal))
				return WordBoundaryCondition.BeginningOfSearchItem;

            return srchItemPattern.EndsWith("#", StringComparison.Ordinal) ?
				WordBoundaryCondition.EndOfSearchItem : WordBoundaryCondition.NoCondition;
		}

		/// ------------------------------------------------------------------------------------
		public ZeroOrMoreCondition GetZeroOrMoreCondition()
		{
			string tmpSrchItem = RemoveDiacriticPlaceholderModifiers(m_srchItemStr);
			string tmpEnvBefore = RemoveDiacriticPlaceholderModifiers(m_envBeforeStr);
			string tmpEnvAfter = RemoveDiacriticPlaceholderModifiers(m_envAfterStr);
            tmpSrchItem = tmpSrchItem.Replace("NOT", "");
		    tmpEnvBefore = tmpEnvBefore.Replace("NOT", "");
            tmpEnvAfter = tmpEnvAfter.Replace("NOT", "");
			// Check search item
			if (tmpSrchItem.Contains("*"))
				return ZeroOrMoreCondition.InSearchItem;

			// Check environment before
			string[] pieces = tmpEnvBefore.Split('*');

			if (pieces.Length > 2)
			    return ZeroOrMoreCondition.MoreThanOneInEnvBefore;

            if (pieces.Length > 1 && !tmpEnvBefore.StartsWith("*", StringComparison.Ordinal))
				return ZeroOrMoreCondition.NotBeginningOfEnvBefore;

			// Check environment after
			pieces = tmpEnvAfter.Split('*');

			if (pieces.Length > 2)
				return ZeroOrMoreCondition.MoreThanOneInEnvAfter;

            if (pieces.Length > 1 && !tmpEnvAfter.EndsWith("*", StringComparison.Ordinal))
				return ZeroOrMoreCondition.NotEndOfEnvAfter;

			return ZeroOrMoreCondition.NoCondition;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether or not there is an invalid word boundary symbol
		/// found in the search pattern, and where it is.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public OneOrMoreCondition GetOneOrMoreCondition()
		{
			string tmp = RemovePlusBinaryFeatures(m_srchItemStr);
			tmp = RemoveDiacriticPlaceholderModifiers(tmp);

			// Check search item
			if (tmp.Contains("+"))
				return OneOrMoreCondition.InSearchItem;

			// Check environment before
			tmp = RemovePlusBinaryFeatures(m_envBeforeStr);
		    tmp = tmp.Replace("not", "");
			string[] pieces = tmp.Split("+".ToCharArray());

			if (pieces.Length > 2)
				return OneOrMoreCondition.MoreThanOneInEnvBefore;

            if (pieces.Length > 1 && !tmp.StartsWith("+", StringComparison.Ordinal))
				return OneOrMoreCondition.NotBeginningOfEnvBefore;

			// Check environment after
			tmp = RemovePlusBinaryFeatures(m_envAfterStr);
            tmp = tmp.Replace("not", "");
			pieces = tmp.Split("+".ToCharArray());

			if (pieces.Length > 2)
				return OneOrMoreCondition.MoreThanOneInEnvAfter;

            if (pieces.Length > 1 && !tmp.EndsWith("+", StringComparison.Ordinal))
				return OneOrMoreCondition.NotEndOfEnvAfter;

			return OneOrMoreCondition.NoCondition;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get rid of all +binary feature names in the specified string.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static string RemovePlusBinaryFeatures(string pattern)
		{
			pattern = pattern.ToLower();

			foreach (var feature in App.BFeatureCache.Values)
			{
				// Remove those whose short name was specified.
				var ptrnFeature = string.Format("[{0}]", feature.Name.ToLower());
				pattern = pattern.Replace(ptrnFeature, string.Empty);

				// Remove those whose full name was specified.
				ptrnFeature = string.Format("[{0}]", feature.FullName.ToLower());
				pattern = pattern.Replace(ptrnFeature, string.Empty);
			}

			return pattern;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Get rid of all the stuff between diacritic placeholder (i.e. dotted circle) and
		/// its enclosing closed square bracket.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private static string RemoveDiacriticPlaceholderModifiers(string pattern)
		{
			int start = 0;

			while ((start = pattern.IndexOf(App.DottedCircleC, start)) >= 0)
			{
				int end = pattern.IndexOf(']', start);
				if (end < start)
					break;

				start++;
				pattern = pattern.Remove(start, end - start);
			}

			return pattern;
		}

		#endregion

		///// ------------------------------------------------------------------------------------
		//private static string GetPhonesFromMember(PatternGroup grp)
		//{
		//    var phones = new StringBuilder();

		//    if (grp == null)
		//        return string.Empty;

		//    foreach (object obj in grp.Members)
		//    {
		//        if (obj is PatternGroup)
		//            phones.Append(GetPhonesFromMember(obj as PatternGroup));
		//        else
		//        {
		//            var member = obj as PatternGroupMember;
		//            if (member != null && member.Member != null &&
		//                member.Member.Trim() != string.Empty &&
		//                member.MemberType == MemberType.SinglePhone)
		//            {
		//                phones.Append(member.Member.Trim());
		//            }

		//            //PatternGroupMember member = obj as PatternGroupMember;
		//            //if (member != null && member.Member != null &&
		//            //    member.Member.Trim() != string.Empty &&
		//            //    (member.MemberType == MemberType.SinglePhone ||
		//            //    member.MemberType == MemberType.IPACharacterRun))
		//            //{
		//            //    phones.Append(member.Member.Trim());
		//            //}
		//        }
		//    }

		//    return phones.ToString();
		//}

		///// ------------------------------------------------------------------------------------
		//private static List<char> GetInvalidCharsFromMember(PatternGroup grp)
		//{
		//    var undefinedChars = new List<char>();

		//    if (grp != null)
		//    {
		//        foreach (object obj in grp.Members)
		//        {
		//            if (obj is PatternGroup)
		//                undefinedChars.AddRange(GetInvalidCharsFromMember(obj as PatternGroup));
		//            else
		//            {
		//                var member = obj as PatternGroupMember;
		//                if (member != null)
		//                    undefinedChars.AddRange(member.UndefinedPhoneticChars);
		//            }
		//        }

		//    }
		
		//    return undefinedChars;
		//}

		#region Diacritic Pattern comparer used by pattern group members and pattern groups.
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Parses a phone into its base portion and its diacritics and returns a list of
		/// the characters that are not found in the phonetic character inventory, if any.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public static List<char> ParsePhone(string phone, out string basePhone, out string diacritics)
		{
			// First, check if the phone is a tone letter.
			if (App.IPASymbolCache.ToneLetterInfo(phone) != null)
			{
				basePhone = phone;
				diacritics = null;
				return null;
			}

			var sbBasePhone = new StringBuilder();
			var sbDiacritics = new List<char>(5);
			var undefinedChars = new List<char>();
			bool tiebarFound = false;

			foreach (char c in phone)
			{
				var charInfo = App.IPASymbolCache[c];

				// This should never be null.
				if (charInfo == null || charInfo.IsUndefined)
					undefinedChars.Add(c);
				
				if (charInfo != null)
				{
					// Tie bars are counted as part of the base character.
					if (charInfo.IsBase || c == App.kBottomTieBarC || c == App.kTopTieBarC)
					{
						sbBasePhone.Append(c);
						if (!tiebarFound && (c == App.kBottomTieBarC || c == App.kTopTieBarC))
							tiebarFound = true;
					}
					else
					{
						// The check will make sure we don't add duplicate diacritic marks to the
						// list which could happen if both characters under (or over) a tiebar are
						// modified with the same diacritic.
						if (!tiebarFound || !sbDiacritics.Contains(c))
							sbDiacritics.Add(c);
					}
				}
			}

			basePhone = sbBasePhone.ToString();
			diacritics = (sbDiacritics.Count == 0 ? null : new string(sbDiacritics.ToArray()));
			return (undefinedChars.Count == 0 ? null : undefinedChars);
		}
		
		#endregion

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the specified phonetic character array for pattern matches.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool SearchWord(string phonetic, out int[] result)
		{
			result = new[] { -1, -1 };

			if (App.IPASymbolCache == null)
				return false;

			m_matchIndex = 0;
			return SearchWord(App.Project.PhoneticParser.Parse(phonetic, true), out result);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches the specified phonetic character array for pattern matches.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool SearchWord(string[] eticChars, out int[] result)
		{
			m_phones = eticChars;
			m_matchIndex = 0;
			return SearchWord(out result);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Searches a previously specified word or array of phonetic characters for a pattern
		/// match.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public bool SearchWord(out int[] result)
		{
			result = new[] {-1, -1};

			if (m_phones == null)
				return false;

			while (m_matchIndex < m_phones.Length)
			{
				result = new[] { -1, -1 };

				// First, look for the search item.
				if (SrchItemPatternGroup == null || !SrchItemPatternGroup.Search(m_phones, m_matchIndex, out result))
					return false;

				// Save where the match was found.
				m_matchIndex = result[0];

				// Now search before the match and after the match to
				// see if we match on the environment before and after.
				if (EnvBeforePatternGroup.Search(m_phones, m_matchIndex - 1))
				{
					if (EnvAfterPatternGroup.Search(m_phones, m_matchIndex + result[1]))
					{
						m_matchIndex++;
						return true;
					}
				}

				m_matchIndex++;
			}

			return false;
		}
	}
}
