namespace Tradie.Common.Services;

/// <summary>
/// Indicates the health of an available service instance.
/// </summary>
public enum ServiceHealth {
	/// <summary>
	/// No health status is known for the instance, or it did not report one.
	/// </summary>
	Unknown = 0,
	/// <summary>
	/// The instance reported an unhealthy status.
	/// </summary>
	Unhealthy = 1,
	/// <summary>
	/// The instance reported a healthy status.
	/// </summary>
	Healthy = 2
}

/// <summary>
/// Provides information about a registered service to discover.
/// </summary>
/// <param name="Endpoint">An endpoint to access the service at.</param>
/// <param name="InstanceId">A unique identifier for this instance.</param>
/// <param name="CustomAttributes">Any additional attributes to identify this instance of the service with, such as league.</param>
public readonly record struct ServiceProperties(
	string Endpoint,
	string InstanceId,
	Dictionary<string, string> CustomAttributes
);

/// <summary>
/// A service that has been registered with the service discovery system. 
/// </summary>
/// <param name="Endpoint">An endpoint to access the service at.</param>
/// <param name="InstanceId">A unique identifier for this instance.</param>
/// <param name="HealthStatus">Indicates whether the service is currently healthy.</param>
public readonly record struct AvailableService(
	string Endpoint,
	string InstanceId,
	ServiceHealth HealthStatus
);