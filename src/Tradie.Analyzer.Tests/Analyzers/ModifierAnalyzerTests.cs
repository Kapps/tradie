using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tradie.Analyzer.Analyzers;
using Tradie.Analyzer.Analyzers.Conversions;
using Tradie.TestUtils;

namespace Tradie.Analyzer.Tests.Analyzers; 

[TestClass]
public class ModifierAnalyzerTests : TestBase {

	[TestInitialize]
	public void Initializer() {
		this._analyzer = new ModifierAnalyzer(this._converter.Object, this._pseudoModCalculator.Object);
	}

	private ModifierAnalyzer _analyzer = null!;
	private Mock<IModConverter> _converter = null!;
	private Mock<IPseudoModCalculator> _pseudoModCalculator = null!;
}