using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradie.Analyzer.Entities;
using Tradie.Common;

namespace Tradie.Analyzer.Models;
/// <summary>
/// Data class containing the various predefined pseudo mods.
/// </summary>
public static class PseudoMods {
	public static Modifier TotalMaxLife = Create("+# Total Maximum Life");
	public static Modifier TotalResists = Create("+# Total Resistances");
	public static Modifier TotalEleRes = Create("+# Total Elemental Resistances");
	public static Modifier TotalChaosRes = Create("+# Total Chaos Resistance");

	public static Modifier[] AllPseudoModifiers => new[] {
		TotalMaxLife, TotalResists, TotalEleRes, TotalChaosRes
	};

	private static Modifier Create(string text) {
		var hash = ModifierText.CalculateValueIndependentHash(text);
		return new Modifier(hash, text, ModifierKind.Pseudo) {
			Id = 1_000_000_000 + (int)(hash % (int.MaxValue - 1_000_000_000))
		};
	}
}