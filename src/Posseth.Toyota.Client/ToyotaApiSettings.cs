using Posseth.Toyota.Client.Const;

namespace Posseth.Toyota.Client;

/// <summary>
/// Holds the Toyota Connected Services API endpoints and client settings that were
/// previously hard-wired to a physical <c>config.json</c> file.
/// </summary>
/// <remarks>
/// <para>
/// Every value has a built-in default, so the library works out of the box. To override
/// the defaults, bind this class from configuration (see <see cref="SectionName"/>) or
/// configure it inline via the dependency-injection extensions.
/// </para>
/// <code>
/// "ToyotaApi": {
///   "ApiBaseUrl": "HTTPS://...",
///   "ApiKey": "..."
/// }
/// </code>
/// </remarks>
public sealed class ToyotaApiSettings
{
    /// <summary>
    /// The configuration section name used when binding from <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    public const string SectionName = "ToyotaApi";

    /// <summary>
    /// The client/app version reported in request headers.
    /// </summary>
    public string ClientVersion { get; set; } = Constants.CLIENT_VERSION;

    /// <summary>
    /// Base URL of the Toyota Connected Services (MyT) API.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "HTTPS://ctpa-oneapi.tceu-ctp-prd.toyotaconnectedeurope.io";

    /// <summary>
    /// OAuth 2.0 access-token endpoint.
    /// </summary>
    public string AccessTokenUrl { get; set; } = "HTTPS://b2c-login.toyota-europe.com/oauth2/realms/root/realms/tme/access_token";

    /// <summary>
    /// ForgeRock AM authentication endpoint.
    /// </summary>
    public string AuthenticateUrl { get; set; } =
        "HTTPS://b2c-login.toyota-europe.com/json/realms/root/realms/tme/authenticate?authIndexType=service&authIndexValue=oneapp";

    /// <summary>
    /// OAuth 2.0 authorization endpoint.
    /// </summary>
    public string AuthorizeUrl { get; set; } =
        "HTTPS://b2c-login.toyota-europe.com/oauth2/realms/root/realms/tme/authorize?client_id=oneapp&scope=openid+profile+write&response_type=code&redirect_uri=com.toyota.oneapp:/oauth2Callback&code_challenge=plain&code_challenge_method=plain";

    /// <summary>
    /// Customer account endpoint.
    /// </summary>
    public string CustomerAccountEndpoint { get; set; } = "TBD";

    public string VehicleAssociationEndpoint { get; set; } = "/v1/vehicle-association/vehicle";
    public string VehicleGuidEndpoint { get; set; } = "/v2/vehicle/guid";
    public string VehicleLocationEndpoint { get; set; } = "/v1/location";
    public string VehicleHealthStatusEndpoint { get; set; } = "/v1/vehiclehealth/status";
    public string VehicleGlobalRemoteStatusEndpoint { get; set; } = "/v1/global/remote/status";
    public string VehicleGlobalRemoteElectricStatusEndpoint { get; set; } = "/v1/global/remote/electric/status";
    public string VehicleGlobalRemoteElectricRealtimeStatusEndpoint { get; set; } = "/v1/global/remote/electric/realtime-status";
    public string VehicleTelemetryEndpoint { get; set; } = "/v3/telemetry";
    public string VehicleNotificationHistoryEndpoint { get; set; } = "/v2/notification/history";

    /// <summary>
    /// Trip-history endpoint template. Supports the placeholders
    /// <c>{from_date}</c>, <c>{to_date}</c>, <c>{route}</c>, <c>{summary}</c>, <c>{limit}</c> and <c>{offset}</c>.
    /// </summary>
    public string VehicleTripsEndpoint { get; set; } =
        "/v1/trips?from={from_date}&to={to_date}&route={route}&summary={summary}&limit={limit}&offset={offset}";

    public string VehicleServiceHistoryEndpoint { get; set; } = "/v1/servicehistory/vehicle/summary";
    public string VehicleClimateControlEndpoint { get; set; } = "/v1/global/remote/climate-control";
    public string VehicleClimateSettingsEndpoint { get; set; } = "/v1/global/remote/climate-settings";
    public string VehicleClimateStatusEndpoint { get; set; } = "/v1/global/remote/climate-status";
    public string VehicleClimateStatusRefreshEndpoint { get; set; } = "/v1/global/remote/refresh-climate-status";
    public string VehicleCommandEndpoint { get; set; } = "/v1/global/remote/command";
    public string VehicleDrivingStatisticsEndpoint { get; set; } = "/v1/driving-statistics";
    public string VehicleLockStatusEndpoint { get; set; } = "/v1/global/remote/lock-status";

    /// <summary>
    /// API key sent in the <c>x-api-key</c> request header.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a credential and is intentionally <b>not</b> hard-coded in this library.
    /// Supply it through the <c>ToyotaApi:ApiKey</c> configuration section, the
    /// <c>TOYOTA_API_KEY</c> environment variable, or the inline
    /// <see cref="ToyotaApiSettings"/> options.
    /// </para>
    /// <code>
    /// "ToyotaApi": {
    ///   "ApiKey": "your-api-key"
    /// }
    /// </code>
    /// </remarks>
    public string ApiKey { get; set; } = string.Empty;
}
