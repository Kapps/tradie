namespace Tradie.Common {
	/// <summary>
	/// Helper methods for performing cold start initialization.
	/// </summary>
	public static class Initializers {
		/// <summary>
		/// Initializes this instance by calling the given initializer once and only once.
		/// </summary>
		public static async Task InitializeOnce(Func<Task> initializer) {
			if(_initialized) {
				return;
			}

			await _initializationLock.WaitAsync();
			try {
				if(_initialized) {
					return;
				}

				await initializer();
				
				_initialized = true;
			} finally {
				_initializationLock.Release();
			}
		}

		private static bool _initialized;
		private static SemaphoreSlim _initializationLock = new(1);
	}
}