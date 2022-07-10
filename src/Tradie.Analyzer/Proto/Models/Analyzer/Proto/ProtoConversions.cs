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

public partial class Requirements {
	public static implicit operator Entities.Requirements?(Requirements? requirements) {
		if(requirements == null)
			return default;
		return new(requirements.Dex, requirements.Str, requirements.Int, requirements.Level);
	}
}

public partial class ItemType {
	public static implicit operator Entities.ItemType(ItemType itemType) {
		return new(itemType.Id, itemType.Name, itemType.Category, itemType.Subcategories?.ToArray() ?? Array.Empty<string>(),
			itemType.Requirements, itemType.IconUrl, itemType.Width, itemType.Height);
	}
}