using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Models;

namespace Tradie.Analyzer.Proto;

public partial class Modifier {
	public static implicit operator Entities.Modifier(Modifier modifier) {
		return new((ulong)modifier.Hash, modifier.Text) {
			Id = modifier.Id
		};
	}
}
public partial class ModKey {
	public static implicit operator Analyzers.ModKey(ModKey? modKey) {
		if(modKey == null)
			return default;
		return new((ulong)modKey.Modifier, (ModKind)modKey.Location);
	}
}