﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2005-2015, SIL International.
// <copyright from='2005' to='2015' company='SIL International'>
//		Copyright (c) 2005-2015, SIL International.
//    
//		This software is distributed under the MIT License, as specified in the LICENSE.txt file.
// </copyright> 
#endregion
// 
using System.Xml.Serialization;

namespace SIL.Pa.Model
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// The SortInformation struct holds sort information.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class SortField
	{
		private string m_paFieldName;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This is mainly used for serializing and deserializing.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlAttribute("name")]
		public string PaFieldName
		{
			get { return (Field != null ? Field.Name : m_paFieldName); }
			set { m_paFieldName = value; }
		}

		[XmlAttribute("ascending")]
		public bool Ascending { get; set; }

		[XmlIgnore]
		public PaField Field { get; set; }

		/// ------------------------------------------------------------------------------------
		public SortField()
		{
		}

		/// ------------------------------------------------------------------------------------
		public SortField(PaField field, bool sortDirection)
		{
			Field = field;
			Ascending = sortDirection;
		}

		/// ------------------------------------------------------------------------------------
		public SortField Copy()
		{
			return new SortField
			{
				m_paFieldName = m_paFieldName,
				Ascending = Ascending,
				Field = Field,		// REVIEW: I don't think we want a deep copy of the field, do we?
			};
		}

		/// ------------------------------------------------------------------------------------
		public override string ToString()
		{
			return PaFieldName + ": " + (Ascending ? "Ascending" : "Descending");
		}
	}
}
