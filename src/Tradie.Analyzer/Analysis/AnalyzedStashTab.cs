namespace Tradie.Analyzer;

/// <summary>
/// Represents a stash tab that has been fully analyzed.
/// </summary>
public record struct AnalyzedStashTab(string StashTabId, AnalyzedItem[] Items);
