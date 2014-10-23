using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using SIL.PaToFdoInterfaces;

namespace SIL.FieldWorks.PaObjects
{
	/// ----------------------------------------------------------------------------------------
	public class PaLexSense : IPaLexSense
	{
		/// ------------------------------------------------------------------------------------
		public PaLexSense()
		{
		}

		/// ------------------------------------------------------------------------------------
		internal PaLexSense(dynamic lxSense)
		{
			var svcloc = lxSense.Cache.ServiceLocator;

			xAnthropologyNote = PaMultiString.Create(lxSense.AnthroNote, svcloc);
			xBibliography = PaMultiString.Create(lxSense.Bibliography, svcloc);
			xDefinition = PaMultiString.Create(lxSense.Definition, svcloc);
			xDiscourseNote = PaMultiString.Create(lxSense.DiscourseNote, svcloc);
			xEncyclopedicInfo = PaMultiString.Create(lxSense.EncyclopedicInfo, svcloc);
			xGeneralNote = PaMultiString.Create(lxSense.GeneralNote, svcloc);
			xGloss = PaMultiString.Create(lxSense.Gloss, svcloc);
			xGrammarNote = PaMultiString.Create(lxSense.GrammarNote, svcloc);
			xPhonologyNote = PaMultiString.Create(lxSense.PhonologyNote, svcloc);
			xRestrictions = PaMultiString.Create(lxSense.Restrictions, svcloc);
			xSemanticsNote = PaMultiString.Create(lxSense.SemanticsNote, svcloc);
			xSociolinguisticsNote = PaMultiString.Create(lxSense.SocioLinguisticsNote, svcloc);

            xReversalEntries = new List<PaMultiString>();
            foreach (var x in lxSense.ReversalEntriesRC)
                xReversalEntries.Add(PaMultiString.Create(x.ReversalForm, svcloc));

			xGuid = lxSense.Guid;

			ImportResidue = lxSense.ImportResidue.Text;
			Source = lxSense.Source.Text;
			ScientificName = lxSense.ScientificName.Text;

            xAnthroCodes = new List<PaCmPossibility>();
            foreach (var x in lxSense.AnthroCodesRC)
                xAnthroCodes.Add(PaCmPossibility.Create(x));

            xDomainTypes = new List<PaCmPossibility>();
            foreach (var x in lxSense.DomainTypesRC)
                xDomainTypes.Add(PaCmPossibility.Create(x));

            xUsages = new List<PaCmPossibility>();
            foreach (var x in lxSense.UsageTypesRC)
                xUsages.Add(PaCmPossibility.Create(x));

            xSemanticDomains = new List<PaCmPossibility>();
            foreach (var x in lxSense.SemanticDomainsRC)
                xSemanticDomains.Add(PaCmPossibility.Create(x));
			
            xStatus = PaCmPossibility.Create(lxSense.StatusRA);
			xSenseType = PaCmPossibility.Create(lxSense.SenseTypeRA);

            // TODO: need to implement this secion
            //ICmPossibility poss = null;
            //var msa = lxSense.MorphoSyntaxAnalysisRA;
            //if (msa is IMoDerivAffMsa)
            //    poss = ((IMoDerivAffMsa)msa).FromPartOfSpeechRA;
            //else if (msa is IMoDerivStepMsa)
            //    poss = ((IMoDerivStepMsa)msa).PartOfSpeechRA;
            //else if (msa is IMoInflAffMsa)
            //    poss = ((IMoInflAffMsa)msa).PartOfSpeechRA;
            //else if (msa is IMoStemMsa)
            //    poss = ((IMoStemMsa)msa).PartOfSpeechRA;
            //else if (msa is IMoUnclassifiedAffixMsa)
            //    poss = ((IMoUnclassifiedAffixMsa)msa).PartOfSpeechRA;

            //if (poss != null)
            //    xPartOfSpeech = PaCmPossibility.Create(poss);
		}

		#region IPaLexSense Members
		/// ------------------------------------------------------------------------------------
		public List<PaCmPossibility> xAnthroCodes { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IEnumerable<IPaCmPossibility> AnthroCodes
		{
			get { return xAnthroCodes.Select(x => (IPaCmPossibility)x); }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xAnthropologyNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString AnthropologyNote
		{
			get { return xAnthropologyNote; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xBibliography { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString Bibliography
		{
			get { return xBibliography; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xDefinition { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString Definition
		{
			get { return xDefinition; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xDiscourseNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString DiscourseNote
		{
			get { return xDiscourseNote; }
		}

		/// ------------------------------------------------------------------------------------
		public List<PaCmPossibility> xDomainTypes { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IEnumerable<IPaCmPossibility> DomainTypes
		{
			get { return xDomainTypes.Select(x => (IPaCmPossibility)x); }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xEncyclopedicInfo { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString EncyclopedicInfo
		{
			get { return xEncyclopedicInfo; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xGeneralNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString GeneralNote
		{
			get { return xGeneralNote; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xGloss { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString Gloss
		{
			get { return xGloss; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xGrammarNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString GrammarNote
		{
			get { return xGrammarNote; }
		}

		/// ------------------------------------------------------------------------------------
		public string ImportResidue { get; set; }

		/// ------------------------------------------------------------------------------------
		public PaCmPossibility xPartOfSpeech { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaCmPossibility PartOfSpeech
		{
			get { return xPartOfSpeech; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xPhonologyNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString PhonologyNote
		{
			get { return xPhonologyNote; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xRestrictions { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString Restrictions
		{
			get { return xRestrictions; }
		}

		/// ------------------------------------------------------------------------------------
		public string ScientificName { get; set; }

		/// ------------------------------------------------------------------------------------
		public List<PaCmPossibility> xSemanticDomains { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IEnumerable<IPaCmPossibility> SemanticDomains
		{
			get { return xSemanticDomains.Select(x => (IPaCmPossibility)x); }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xSemanticsNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString SemanticsNote
		{
			get { return xSemanticsNote; }
		}

		/// ------------------------------------------------------------------------------------
		public PaCmPossibility xSenseType { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaCmPossibility SenseType
		{
			get { return xSenseType; }
		}

		/// ------------------------------------------------------------------------------------
		public PaMultiString xSociolinguisticsNote { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaMultiString SociolinguisticsNote
		{
			get { return xSociolinguisticsNote; }
		}

		/// ------------------------------------------------------------------------------------
		public string Source { get; set; }

		/// ------------------------------------------------------------------------------------
		public PaCmPossibility xStatus { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IPaCmPossibility Status
		{
			get { return xStatus; }
		}

		/// ------------------------------------------------------------------------------------
		public List<PaCmPossibility> xUsages { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IEnumerable<IPaCmPossibility> Usages
		{
			get { return xUsages.Select(x => (IPaCmPossibility)x); }
		}

		/// ------------------------------------------------------------------------------------
		public List<PaMultiString> xReversalEntries { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public IEnumerable<IPaMultiString> ReversalEntries
		{
			get { return xReversalEntries.Select(x => (IPaMultiString)x); }
		}

		/// ------------------------------------------------------------------------------------
		public Guid xGuid { get; set; }

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public Guid Guid
		{
			get { return xGuid; }
		}

		#endregion
	}
}
