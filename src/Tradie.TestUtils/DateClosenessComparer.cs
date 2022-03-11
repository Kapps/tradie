using DeepEqual;

namespace Tradie.TestUtils;

public class DateClosenessComparer : IComparison {
	public bool CanCompare(Type type1, Type type2) {
		return type1 == typeof(DateTime) && type2 == typeof(DateTime);
	}

	public (ComparisonResult result, IComparisonContext context) Compare(IComparisonContext context, object value1, object value2) {
		var dt1 = (DateTime)value1;
		var dt2 = (DateTime)value2;

		bool match = Math.Abs(dt1.Ticks - dt2.Ticks) < (TimeSpan.TicksPerMillisecond * 200);
		var compared = match ? ComparisonResult.Pass : ComparisonResult.Fail;

		return (compared, context);
	}
}