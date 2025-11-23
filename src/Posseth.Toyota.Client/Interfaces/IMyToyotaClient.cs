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
    }
}