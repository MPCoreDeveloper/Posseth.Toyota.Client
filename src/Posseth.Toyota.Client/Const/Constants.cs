using System;
using Microsoft.Extensions.Configuration;

namespace Posseth.Toyota.Client.Const
{
    public static class Constants
    {
        private static readonly IConfiguration _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .Build();

        // Misc
        public const string CLIENT_VERSION = "2.14.0";

        // Units
        public const string KILOMETERS_UNIT = "km";
        public const string MILES_UNIT = "mi";
        public const double L_TO_MPG_FACTOR = 235.215;
        public const double ML_TO_L_FACTOR = 1000.0;
        public const double ML_TO_GAL_FACTOR = 3785.0;
        public const double KM_TO_MILES_FACTOR = 0.621371192;
        public const double MILES_TO_KM_FACTOR = 1.60934;

        // API URLs
        public static readonly string API_BASE_URL = _configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not found in configuration.");
        public static readonly string ACCESS_TOKEN_URL = _configuration["AccessTokenUrl"] ?? throw new InvalidOperationException("AccessTokenUrl not found in configuration.");
        public static readonly string AUTHENTICATE_URL = _configuration["AuthenticateUrl"] ?? throw new InvalidOperationException("AuthenticateUrl not found in configuration.");
        public static readonly string AUTHORIZE_URL = _configuration["AuthorizeUrl"] ?? throw new InvalidOperationException("AuthorizeUrl not found in configuration.");

        // Endpoint URLs
        public static readonly string CUSTOMER_ACCOUNT_ENDPOINT = _configuration["CustomerAccountEndpoint"] ?? throw new InvalidOperationException("CustomerAccountEndpoint not found in configuration.");
        public static readonly string VEHICLE_ASSOCIATION_ENDPOINT = _configuration["VehicleAssociationEndpoint"] ?? throw new InvalidOperationException("VehicleAssociationEndpoint not found in configuration.");
        public static readonly string VEHICLE_GUID_ENDPOINT = _configuration["VehicleGuidEndpoint"] ?? throw new InvalidOperationException("VehicleGuidEndpoint not found in configuration.");
        public static readonly string VEHICLE_LOCATION_ENDPOINT = _configuration["VehicleLocationEndpoint"] ?? throw new InvalidOperationException("VehicleLocationEndpoint not found in configuration.");
        public static readonly string VEHICLE_HEALTH_STATUS_ENDPOINT = _configuration["VehicleHealthStatusEndpoint"] ?? throw new InvalidOperationException("VehicleHealthStatusEndpoint not found in configuration.");
        public static readonly string VEHICLE_GLOBAL_REMOTE_STATUS_ENDPOINT = _configuration["VehicleGlobalRemoteStatusEndpoint"] ?? throw new InvalidOperationException("VehicleGlobalRemoteStatusEndpoint not found in configuration.");
        public static readonly string VEHICLE_GLOBAL_REMOTE_ELECTRIC_STATUS_ENDPOINT = _configuration["VehicleGlobalRemoteElectricStatusEndpoint"] ?? throw new InvalidOperationException("VehicleGlobalRemoteElectricStatusEndpoint not found in configuration.");
        public static readonly string VEHICLE_GLOBAL_REMOTE_ELECTRIC_REALTIME_STATUS_ENDPOINT = _configuration["VehicleGlobalRemoteElectricRealtimeStatusEndpoint"] ?? throw new InvalidOperationException("VehicleGlobalRemoteElectricRealtimeStatusEndpoint not found in configuration.");
        public static readonly string VEHICLE_TELEMETRY_ENDPOINT = _configuration["VehicleTelemetryEndpoint"] ?? throw new InvalidOperationException("VehicleTelemetryEndpoint not found in configuration.");
        public static readonly string VEHICLE_NOTIFICATION_HISTORY_ENDPOINT = _configuration["VehicleNotificationHistoryEndpoint"] ?? throw new InvalidOperationException("VehicleNotificationHistoryEndpoint not found in configuration.");
        public static readonly string VEHICLE_TRIPS_ENDPOINT = _configuration["VehicleTripsEndpoint"] ?? throw new InvalidOperationException("VehicleTripsEndpoint not found in configuration.");
        public static readonly string VEHICLE_SERVICE_HISTORY_ENDPOINT = _configuration["VehicleServiceHistoryEndpoint"] ?? throw new InvalidOperationException("VehicleServiceHistoryEndpoint not found in configuration.");
        public static readonly string VEHICLE_CLIMATE_CONTROL_ENDPOINT = _configuration["VehicleClimateControlEndpoint"] ?? throw new InvalidOperationException("VehicleClimateControlEndpoint not found in configuration.");
        public static readonly string VEHICLE_CLIMATE_SETTINGS_ENDPOINT = _configuration["VehicleClimateSettingsEndpoint"] ?? throw new InvalidOperationException("VehicleClimateSettingsEndpoint not found in configuration.");
        public static readonly string VEHICLE_CLIMATE_STATUS_ENDPOINT = _configuration["VehicleClimateStatusEndpoint"] ?? throw new InvalidOperationException("VehicleClimateStatusEndpoint not found in configuration.");
        public static readonly string VEHICLE_CLIMATE_STATUS_REFRESH_ENDPOINT = _configuration["VehicleClimateStatusRefreshEndpoint"] ?? throw new InvalidOperationException("VehicleClimateStatusRefreshEndpoint not found in configuration.");
        public static readonly string VEHICLE_COMMAND_ENDPOINT = _configuration["VehicleCommandEndpoint"] ?? throw new InvalidOperationException("VehicleCommandEndpoint not found in configuration.");
        public static readonly string VEHICLE_DRIVING_STATISTICS_ENDPOINT = _configuration["VehicleDrivingStatisticsEndpoint"] ?? throw new InvalidOperationException("VehicleDrivingStatisticsEndpoint not found in configuration.");
        public static readonly string VEHICLE_LOCK_STATUS_ENDPOINT = _configuration["VehicleLockStatusEndpoint"] ?? throw new InvalidOperationException("VehicleLockStatusEndpoint not found in configuration.");

        // API Key
        public static readonly string API_KEY = _configuration["ApiKey"] ?? throw new InvalidOperationException("ApiKey not found in configuration.");
    }
}

