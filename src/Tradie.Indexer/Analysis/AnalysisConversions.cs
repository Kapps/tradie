namespace Tradie.Proto.Analysis;

// TODO: Figure out where to move this kind of stuff. Should there be a Tradie.Analysis.Client?

public partial class ModKey {
	public static implicit operator Analyzer.Analyzers.ModKey(ModKey key) {
		return new((ulong)key.Modifier, (Analyzer.Models.ModKind)key.Location);
	}
}