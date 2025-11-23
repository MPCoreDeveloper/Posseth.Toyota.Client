using Posseth.Toyota.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Posseth.Toyota.Client.Interfaces
{
    public interface IToyotaRestClient
    {
        Task<ToyotaApiResponse?> GetVehicleInfoAsync(string vin, CancellationToken cancellationToken = default);
    }
}