using System.Threading.Tasks;

namespace Tradie.Infrastructure.Packaging {
	public interface IBuilder<T> where T : BuildResult {
		Task<T> Build(string baseDirectory, ResourceConfig config);
	}
	
	public class BuildResult {
		/// <summary>
		/// A hash of the input files used for the build.
		/// </summary>
		public string Hash { get; set; }
	}
}