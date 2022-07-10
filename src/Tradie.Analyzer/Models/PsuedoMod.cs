namespace Tradie.Analyzer.Models;

// TODO: Eventually use this instead of hard-coding the calculations.

/// <summary>
/// Provides information about how to calculate a pseudo mod from one or more other modifiers.
/// Not all pseudo modifiers can be calculated in this way.
/// </summary>
public readonly record struct PsuedoModCalculation(
	ulong ModHash,
	PseudoCalculationKind CalculationKind,
	PseudoModInput[] Inputs
);

public readonly record struct PseudoModInput(
	ulong ModHash,
	float Weight
);

public enum PseudoCalculationKind {
	Sum,
	Count
}