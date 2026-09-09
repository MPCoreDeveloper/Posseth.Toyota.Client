using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Posseth.Toyota.Client;
using Posseth.Toyota.Client.Const;
using Posseth.Toyota.Client.Extensions;
using Posseth.Toyota.Client.Interfaces;
using Posseth.Toyota.Client.Models;
using Posseth.Toyota.Client.Services;
using Xunit;

namespace Posseth.Toyota.Client.Tests;

/// <summary>
/// Offline unit tests. These never hit the Toyota servers and therefore do not
/// require the TOYOTA_USERNAME / TOYOTA_PASSWORD environment variables.
/// </summary>
public class OfflineUnitTests
{
    [Fact]
    public void ToyotaApiSettings_HasBuiltInDefaults()
    {
        var settings = new ToyotaApiSettings();

        Assert.False(string.IsNullOrWhiteSpace(settings.ApiBaseUrl));
        // SECURITY: The API key is a credential and has no built-in default anymore.
        Assert.Empty(settings.ApiKey);
        Assert.False(string.IsNullOrWhiteSpace(settings.AccessTokenUrl));
        Assert.False(string.IsNullOrWhiteSpace(settings.AuthorizeUrl));
        Assert.False(string.IsNullOrWhiteSpace(settings.AuthenticateUrl));
        Assert.StartsWith("/v2/vehicle/guid", settings.VehicleGuidEndpoint);
        Assert.Contains("{from_date}", settings.VehicleTripsEndpoint);
        Assert.Contains("{offset}", settings.VehicleTripsEndpoint);
        Assert.Equal(Constants.CLIENT_VERSION, settings.ClientVersion);
    }

    [Fact]
    public void ToyotaApiSettings_ApiKey_CanBeConfigured()
    {
        // SECURITY: The API key has no built-in default; it must be supplied by the application,
        // either through ToyotaApiSettings directly or via configuration.
        var settings = new ToyotaApiSettings { ApiKey = "configured-key" };
        Assert.Equal("configured-key", settings.ApiKey);

        settings.ApiKey = "rotated-key";
        Assert.Equal("rotated-key", settings.ApiKey);
    }

    [Fact]
    public void ToyotaClientOptions_HasSecureDefaults()
    {
        var options = new ToyotaClientOptions();

        Assert.Equal(60, options.TimeoutSeconds);
        Assert.True(options.UseTokenCaching);
        Assert.False(options.BypassSslValidation);
        Assert.False(string.IsNullOrWhiteSpace(options.TokenCacheFilename));
    }

    [Fact]
    public void AddToyotaClient_RegistersTransientClient()
    {
        var services = new ServiceCollection();
        services.AddToyotaClient(options =>
        {
            options.Username = "test@example.com";
            options.Password = "test-password";
        });

        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<IMyToyotaClient>();
        var second = provider.GetRequiredService<IMyToyotaClient>();

        Assert.IsType<MyToyotaClient>(first);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void AddToyotaClient_WithNullArguments_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddToyotaClient(_ => { }));
        Assert.Throws<ArgumentNullException>(() =>
            new ServiceCollection().AddToyotaClient((Action<ToyotaClientOptions>)null!));
    }

    [Fact]
    public void SslValidation_IsOffByDefault_AndCanBeToggled()
    {
        var handler = new HttpClientHandler();
        var client = new MyToyotaClient(new ToyotaApiSettings(), handler);

        // Secure by default: no dangerous callback installed.
        Assert.Null(handler.ServerCertificateCustomValidationCallback);

        client.UseBypassSslValidation(true);
        Assert.NotNull(handler.ServerCertificateCustomValidationCallback);

        client.UseBypassSslValidation(false);
        Assert.Null(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void TokenCacheItem_RoundTripsThroughJson()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var original = new TokenCacheItem
        {
            access_token = "access",
            refresh_token = "refresh",
            uuid = "00000000-0000-0000-0000-000000000000",
            expiration = DateTime.UtcNow.AddHours(1),
            username = "test@example.com"
        };

        var json = JsonSerializer.Serialize(original, options);
        var roundTripped = JsonSerializer.Deserialize<TokenCacheItem>(json, options);

        Assert.NotNull(roundTripped);
        Assert.Equal(original.access_token, roundTripped!.access_token);
        Assert.Equal(original.refresh_token, roundTripped.refresh_token);
        Assert.Equal(original.uuid, roundTripped.uuid);
        Assert.Equal(original.username, roundTripped.username);
        Assert.Equal(original.expiration, roundTripped.expiration);
    }
}
