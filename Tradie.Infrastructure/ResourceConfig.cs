namespace Tradie.Infrastructure {
	public class ResourceConfig {
		/// <summary>
		/// Environment is the environment prefix for this deploy, such as tradie-ca.
		/// </summary>
		public string Environment { get; set; }
		/// <summary>
		/// The region code that this deploy is running on, such as us-east-1.
		/// </summary>
		public string Region { get; set; }
		/// <summary>
		/// An arbitrary version or build number.
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// The compile-time base directory for the solution file.
		/// </summary>
		public string BaseDirectory { get; set; }
		/// <summary>
		/// Password for the admin account for the database.
		/// </summary>
		public string DbPassword { get; set; }
	}
}