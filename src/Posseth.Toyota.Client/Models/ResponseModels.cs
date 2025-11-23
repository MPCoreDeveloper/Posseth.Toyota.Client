namespace Posseth.Toyota.Client.Models
{
    public class ElectricResponseModel
    {
        public Status? status { get; set; }
        public ElectricPayload? payload { get; set; }
    }

    public class ElectricPayload
    {
        public string? chargingStatus { get; set; }
        public int batteryLevel { get; set; }
        public RangeValue? evRange { get; set; }
        public RangeValue? evRangeWithAc { get; set; }
    }

    public class RangeValue
    {
        public object? value { get; set; } // Changed to object? to handle string/double
        public string? unit { get; set; }
    }

    /// <summary>
    /// Represents the detailed status of a realtime electric status request
    /// </summary>
    public record RealtimeStatus
    {
        /// <summary>
        /// Detailed status information including response messages
        /// </summary>
        public RealtimeStatusStatus? status { get; init; }

        /// <summary>
        /// Payload containing request information
        /// </summary>
        public RealtimeStatusPayload? payload { get; init; }
    }

    /// <summary>
    /// Contains status information about the realtime request
    /// </summary>
    public record RealtimeStatusStatus
    {
        /// <summary>
        /// Service identifier for the request
        /// </summary>
        public object? serviceIdentifier { get; init; }

        /// <summary>
        /// Collection of messages related to the status
        /// </summary>
        public List<RealtimeStatusMessage> messages { get; init; } = [];
    }

    /// <summary>
    /// Contains detailed message information for a realtime status response
    /// </summary>
    public record RealtimeStatusMessage
    {
        /// <summary>
        /// Response code from the service
        /// </summary>
        public string? responseCode { get; init; }

        /// <summary>
        /// Brief description of the message
        /// </summary>
        public string? description { get; init; }

        /// <summary>
        /// Detailed description of the message
        /// </summary>
        public string? detailedDescription { get; init; }
    }

    /// <summary>
    /// Contains payload information for a realtime status request
    /// </summary>
    public record RealtimeStatusPayload
    {
        /// <summary>
        /// Application request number for tracking
        /// </summary>
        public string? appRequestNo { get; init; }

        /// <summary>
        /// Return code indicating the result of the request
        /// </summary>
        public string? returnCode { get; init; }
    }

    public class LocationResponseModel
    {
        public object? status { get; set; } // Changed to object?
        public object? code { get; set; }   // Changed to object?
        public LocationPayload? payload { get; set; }
    }

    public class LocationPayload
    {
        public VehicleLocation? vehicleLocation { get; set; }
    }

    public class VehicleLocation
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string? displayName { get; set; }
        public string? locationAcquisitionDatetime { get; set; }
    }

    public class HealthStatusResponseModel
    {
        public object? status { get; set; } // Changed to object?
        public HealthPayload? payload { get; set; }
    }

    public class HealthPayload
    {
        public List<object> quantityOfEngOilIcon { get; set; } = new List<object>();
        public List<object> warning { get; set; } = new List<object>();
        public string? wnglastUpdTime { get; set; }
    }

    public class TelemetryStatusResponseModel
    {
        public object? status { get; set; } // Changed to object?
        public TelemetryPayload? payload { get; set; }
    }

    public class TelemetryPayload
    {
        public RangeValue? distanceToEmpty { get; set; }
        public RangeValue? odometer { get; set; }
        public string? timestamp { get; set; }
    }

    public class NotificationsResponseModel
    {
        public Status? status { get; set; }
        public List<object>? payload { get; set; }
    }

    public class RemoteStatusResponseModel
    {
        public object? status { get; set; } // Changed to object?
        public RemotePayload? payload { get; set; }
    }

    public class RemotePayload
    {
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? locationAcquisitionDatetime { get; set; }
        public string? occurrenceDate { get; set; }
        public RemoteTelemetry? telemetry { get; set; }
        public List<VehicleStatusItem>? vehicleStatus { get; set; }
    }

    public class RemoteTelemetry
    {
        // This is the key fix: fugage is of type RangeValue?, not object?
        public RangeValue? fugage { get; set; } 
        public RangeValue? odo { get; set; }
        public RangeValue? rage { get; set; }
    }

    public class VehicleStatusItem
    {
        public string? category { get; set; }
        public int? displayOrder { get; set; }
        public List<StatusSection>? sections { get; set; }
    }

    public class StatusSection
    {
        public string? section { get; set; }
        public List<StatusValue>? values { get; set; }
    }

    public class StatusValue
    {
        public string? value { get; set; }
        public int? status { get; set; }
    }

    public class ServiceHistoryResponseModel
    {
        public Status? status { get; set; }
        public ServiceHistoryPayload? payload { get; set; }
    }

    public class ServiceHistoryPayload
    {
        public List<ServiceHistory>? serviceHistories { get; set; }
    }

    public class ServiceHistory
    {
        public string? serviceDate { get; set; }
        public object? mileage { get; set; } // Changed to object?
        public string? unit { get; set; }
        public string? serviceCategory { get; set; }
        public string? serviceProvider { get; set; }
        public string? serviceHistoryId { get; set; }
    }
}