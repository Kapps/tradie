using Tradie.Analyzer.Models;

namespace Tradie.Indexer.Storage;

public unsafe struct AffixRangePart {
	public float MinValue;
	public float MaxValue;
	public ModKind Kind;
	public AffixRangePart* Next;
}