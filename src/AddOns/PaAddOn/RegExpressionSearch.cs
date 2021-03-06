// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005-2015, SIL International.
// <copyright from='2005' to='2015' company='SIL International'>
//		Copyright (c) 2005-2015, SIL International.
//    
//		This software is distributed under the MIT License, as specified in the LICENSE.txt file.
// </copyright> 
#endregion
// 
using System.Diagnostics;
using System.Text.RegularExpressions;
using SIL.Pa.Data;
using SIL.Pa.FFSearchEngine;

namespace SIL.Pa.AddOn
{
	public class RegExpressionSearch
	{
		private readonly SearchQuery m_query;
		private readonly Regex m_regExBefore;
		private readonly Regex m_regExItem;
		private readonly Regex m_regExAfter;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public RegExpressionSearch(SearchQuery query)
		{
			Debug.Assert(query != null);

			m_query = query;

			if (!m_query.IsPatternRegExpression)
				return;

			string[] regEx = query.Pattern.Split(new char[] { DataUtils.kOrc });
			if (regEx.Length == 3)
			{
				m_regExBefore = new Regex(regEx[0]);
				m_regExItem = new Regex(regEx[1]);
				m_regExAfter = new Regex(regEx[1] + regEx[2]);
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and loads a result cache for the specified search query.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public WordListCache Search()
		{
			if (!m_query.IsPatternRegExpression || m_regExBefore == null ||
				m_regExItem == null || m_regExAfter == null)
			{
				return null;
			}

			WordListCache resultCache = new WordListCache();

			foreach (WordCacheEntry wordEntry in PaApp.WordCache)
			{
				int offset = 0;
				string phonetic = wordEntry.PhoneticValueWithPrimaryUncertainty;

				while (offset < phonetic.Length)
				{
					// Find the search item starting at offset.
					Match match = m_regExItem.Match(phonetic, offset);
					if (!match.Success)
						break;

					offset = match.Index;
					int length = match.Length;

					// Search for the environment before, looking for a match that
					// butts up against the match on the search item.
					match = m_regExBefore.Match(phonetic);
					while (match.Success && match.Index + match.Length < offset)
					    match = m_regExBefore.Match(phonetic, match.Index + 1);

					if (match.Success && match.Index + match.Length == offset)
					{
						// Search for the environment after.
						match = m_regExAfter.Match(phonetic, offset);
						if (match.Success && match.Index == offset)
							resultCache.AddEntryFromRegExpSearch(wordEntry, null, offset, length, false);
					}
					
					offset++;
				}
			}

			resultCache.ExtendRegExpMatchesToPhoneBoundaries();
			resultCache.IsForSearchResults = true;
			resultCache.IsForRegExpSearchResults = true;
			return resultCache;
		}
	}
}
