using DeepEqual;

namespace Tradie.TestUtils;

public class StreamComparer : IComparison {
	public bool CanCompare(Type type1, Type type2) {
		return type1.IsSubclassOf(typeof(Stream)) && type2.IsSubclassOf(typeof(Stream));
	}

	public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1, object value2) {
		if(value1 == value2) {
			return (ComparisonResult.Pass, context);
		}
		
		Stream s1 = (Stream)value1;
		Stream s2 = (Stream)value2;

		using var r1 = new StreamReader(s1);
		using var r2 = new StreamReader(s2);

		string res1 = r1.ReadToEnd();
		string res2 = r2.ReadToEnd();

		return (res1 == res2 ? ComparisonResult.Pass : ComparisonResult.Fail, context);
	}
}
