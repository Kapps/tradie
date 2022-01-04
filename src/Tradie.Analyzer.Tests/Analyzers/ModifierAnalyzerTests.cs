using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tradie.Analyzer.Analyzers.Conversions;

namespace Tradie.Analyzer.Tests.Analyzers; 

[TestClass]
public class ModifierAnalyzerTests {
	[TestInitialize]
	public void Initialize() {
		this._converter = new Mock<IModConverter>(MockBehavior.Strict);
	}

	private Mock<IModConverter> _converter;
}