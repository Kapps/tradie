
using Tradie.Analyzer.Entities;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Analyzers.Conversions;

public interface IPseudoModCalculator {
	IEnumerable<Affix> CalculatePseduoMods(AnalyzedItem item, Affix[] affixes);
}

public class PseudoModCalculator : IPseudoModCalculator {
	public IEnumerable<Affix> CalculatePseduoMods(AnalyzedItem item, Affix[] affixes) {
		// Make this less horrible.
		
		double totalLife = 0, totalRes = 0, totalEleRes = 0, totalChaosRes = 0;
		foreach(var affix in affixes) {
			switch(affix.Hash) {
				// Life
				case 9024037547368883040UL:
					totalLife += affix.Scalar;
					break;
				// Str, Str and #
				case 6442787728654046253UL:
				case 13649248616400778709:
				case 5307261403570144686:
					totalLife += affix.Scalar * 0.2;
					break;
				// Single non-Chaos Res
				case 14277321269326717293UL:
				case 7696646984604648615:
				case 12516815037547620767:
					totalRes += affix.Scalar;
					totalEleRes += affix.Scalar;
					break;
				// Dual non-Chaos Res
				case 10070184050534957371:
				case 7408987239217341109:
				case 16548400137695336915:
					totalRes += affix.Scalar * 2;
					totalEleRes += affix.Scalar * 2;
					break;
				// Chaos Res
				case 9119717384377170561:
					totalRes += affix.Scalar;
					totalChaosRes += affix.Scalar;
					break;
				// Chaos and # Res
				case 12269184774600982397:
				case 4749746203377959867:
				case 5910587615841179333:
					totalRes += affix.Scalar * 2;
					totalChaosRes += affix.Scalar;
					totalEleRes += affix.Scalar;
					break;
				// All ele res:
				case 2570102653797474430:
					totalRes += affix.Scalar * 3;
					totalEleRes += affix.Scalar * 3;
					break;
				// All ele res if corrupted
				case 2655376461128714087: 
					var itemDetails = item.Analysis.GetRequired<ItemDetailsAnalysis>(KnownAnalyzers.ItemDetails);
					if(itemDetails.Flags.HasFlag(ItemFlags.Corrupted)) {
						totalRes += affix.Scalar * 3;
						totalEleRes += affix.Scalar * 3;
					}
					break;
			}
		}
		
		if(totalLife > 0)
			yield return Create(PseudoMods.TotalMaxLife, totalLife);
		if(totalRes > 0)
			yield return Create(PseudoMods.TotalResists, totalRes);
		if(totalChaosRes > 0)
			yield return Create(PseudoMods.TotalChaosRes, totalChaosRes);
		if(totalEleRes > 0)
			yield return Create(PseudoMods.TotalEleRes, totalEleRes);
	}

	private static Affix Create(Modifier mod, double value) => new(mod.ModHash, value, ModKind.Pseudo);
}