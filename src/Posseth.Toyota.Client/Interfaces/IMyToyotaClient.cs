using System;
using System.Threading;
using System.Threading.Tasks;
using Posseth.Toyota.Client.Models;

namespace Posseth.Toyota.Client.Interfaces
{
    public interface IMyToyotaClient
    {
        IMyToyotaClient UseCredentials(string username, string password);
        IMyToyotaClient UseLogger(Action<string> logger);
        IMyToyotaClient UseTimeout(int timeoutSeconds);
        IMyToyotaClient UseTokenCacheFilename(string tokenCacheFilename);
        IMyToyotaClient UseTokenCaching(bool useTokenCaching);

        Task<bool> LoginAsync(CancellationToken cancellationToken = default);
        Task<VehiclesModel?> GetVehiclesAsync(CancellationToken cancellationToken = default);
        Task<ElectricResponseModel?> GetElectricAsync(string vin, CancellationToken cancellationToken = default);
        Task<RealtimeStatus?> GetElectricRealtimeStatusAsync(string vin, CancellationToken cancellationToken = default);
        Task<LocationResponseModel?> GetLocationAsync(string vin, CancellationToken cancellationToken = default);
        Task<HealthStatusResponseModel?> GetHealthStatusAsync(string vin, CancellationToken cancellationToken = default);
        Task<TelemetryStatusResponseModel?> GetTelemetryStatusAsync(string vin, CancellationToken cancellationToken = default);
        Task<NotificationsResponseModel?> GetNotificationsAsync(string vin, CancellationToken cancellationToken = default);
        Task<RemoteStatusResponseModel?> GetRemoteStatusAsync(string vin, CancellationToken cancellationToken = default);
        Task<ServiceHistoryResponseModel?> GetServiceHistoryAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves trip history for the vehicle within a date range.
        /// </summary>
        Task<TripsResponseModel?> GetTripsAsync(string vin, DateOnly from, DateOnly to, bool route = false, bool summary = true, int limit = 50, int offset = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current climate settings configured on the vehicle.
        /// </summary>
        Task<ClimateSettingsResponseModel?> GetClimateSettingsAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current climate control status (running/stopped, cabin temperature).
        /// </summary>
        Task<ClimateStatusResponseModel?> GetClimateStatusAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Triggers a fresh climate status request from the vehicle.
        /// </summary>
        Task<ClimateControlResponseModel?> RefreshClimateStatusAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts climate control on the vehicle.
        /// </summary>
        Task<ClimateControlResponseModel?> StartClimateControlAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops climate control on the vehicle.
        /// </summary>
        Task<ClimateControlResponseModel?> StopClimateControlAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a remote command to the vehicle (lock, unlock, engine start/stop, hazard lights, headlights, trunk).
        /// Check <see cref="RemoteServiceCapabilities"/> on the vehicle to verify support before calling.
        /// </summary>
        Task<RemoteCommandResponseModel?> SendRemoteCommandAsync(string vin, RemoteCommandType command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the vehicle association information for the authenticated user.
        /// </summary>
        Task<VehicleAssociationResponseModel?> GetVehicleAssociationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves driving statistics and eco-scores for the vehicle.
        /// </summary>
        Task<DrivingStatisticsResponseModel?> GetDrivingStatisticsAsync(string vin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current lock status for all doors, trunk, and windows.
        /// </summary>
        Task<LockStatusResponseModel?> GetLockStatusAsync(string vin, CancellationToken cancellationToken = default);
    }
}