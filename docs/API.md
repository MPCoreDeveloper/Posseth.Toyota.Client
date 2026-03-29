# API Reference

## IMyToyotaClient Interface

### Configuration Methods

All configuration methods return `IMyToyotaClient` for method chaining (fluent API).

#### UseCredentials
```csharp
IMyToyotaClient UseCredentials(string username, string password)
```
Sets the MyToyota username and password for authentication.

**Parameters:**
- `username` - Your MyToyota username
- `password` - Your MyToyota password

**Example:**
```csharp
client.UseCredentials("myemail@example.com", "mypassword");
```

#### UseLogger
```csharp
IMyToyotaClient UseLogger(Action<string> logger)
```
Provides a custom logging function for debugging and monitoring.

**Parameters:**
- `logger` - Action to receive log messages

**Example:**
```csharp
client.UseLogger(msg => Console.WriteLine($"[Toyota] {msg}"));
```

#### UseTimeout
```csharp
IMyToyotaClient UseTimeout(int timeoutSeconds)
```
Sets the request timeout in seconds.

**Parameters:**
- `timeoutSeconds` - Timeout duration in seconds

**Example:**
```csharp
client.UseTimeout(60);  // 60 second timeout
```

#### UseTokenCaching
```csharp
IMyToyotaClient UseTokenCaching(bool useTokenCaching)
```
Enables or disables token caching to avoid repeated login calls.

**Parameters:**
- `useTokenCaching` - True to enable caching, false to disable

**Example:**
```csharp
client.UseTokenCaching(true);
```

#### UseTokenCacheFilename
```csharp
IMyToyotaClient UseTokenCacheFilename(string tokenCacheFilename)
```
Sets the file path for token caching. Only relevant if `UseTokenCaching(true)`.

**Parameters:**
- `tokenCacheFilename` - File path for storing cached tokens

**Example:**
```csharp
client.UseTokenCacheFilename("~/.cache/toyota_tokens.json");
```

---

## Authentication

### LoginAsync
```csharp
Task<bool> LoginAsync(CancellationToken cancellationToken = default)
```
Authenticates with the MyToyota API using configured credentials.

**Returns:** `true` if successful, `false` otherwise

**Throws:** `ArgumentException` if credentials not configured

**Example:**
```csharp
var success = await client.LoginAsync();
if (!success)
    throw new InvalidOperationException("Failed to authenticate");
```

---

## Vehicle Information

### GetVehiclesAsync
```csharp
Task<VehiclesModel?> GetVehiclesAsync(CancellationToken cancellationToken = default)
```
Retrieves all vehicles associated with the authenticated user.

**Returns:** `VehiclesModel` containing list of vehicles, or null on error

**Example:**
```csharp
var vehicles = await client.GetVehiclesAsync();
foreach (var vehicle in vehicles?.Data ?? [])
{
    Console.WriteLine($"VIN: {vehicle.Vin}, Name: {vehicle.Nickname}");
}
```

### GetVehicleAssociationAsync
```csharp
Task<VehicleAssociationResponseModel?> GetVehicleAssociationAsync(CancellationToken cancellationToken = default)
```
Retrieves the vehicle association information for the authenticated user.

**Returns:** `VehicleAssociationResponseModel` with association details

**Example:**
```csharp
var association = await client.GetVehicleAssociationAsync();
```

---

## Electric/EV Status

### GetElectricAsync
```csharp
Task<ElectricResponseModel?> GetElectricAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves EV battery and charging information.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ElectricResponseModel` with battery and charging status

**Example:**
```csharp
var electric = await client.GetElectricAsync("JTHJP5C27D5012345");
Console.WriteLine($"Battery: {electric?.Data?.BatteryLevel}%");
Console.WriteLine($"Charging: {electric?.Data?.IsCharging}");
```

### GetElectricRealtimeStatusAsync
```csharp
Task<RealtimeStatus?> GetElectricRealtimeStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves real-time EV status data.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `RealtimeStatus` with real-time data

**Example:**
```csharp
var status = await client.GetElectricRealtimeStatusAsync(vin);
```

---

## Location & Lock Status

### GetLocationAsync
```csharp
Task<LocationResponseModel?> GetLocationAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves the current GPS location of the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `LocationResponseModel` with latitude and longitude

**Example:**
```csharp
var location = await client.GetLocationAsync(vin);
Console.WriteLine($"Location: {location?.Data?.Latitude}, {location?.Data?.Longitude}");
```

### GetLockStatusAsync
```csharp
Task<LockStatusResponseModel?> GetLockStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves the lock status of all doors, trunk, and windows.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `LockStatusResponseModel` with lock statuses

**Example:**
```csharp
var lockStatus = await client.GetLockStatusAsync(vin);
Console.WriteLine($"Driver Door: {lockStatus?.Data?.DriverDoor}");
Console.WriteLine($"Trunk: {lockStatus?.Data?.Trunk}");
```

---

## Climate Control

### GetClimateSettingsAsync
```csharp
Task<ClimateSettingsResponseModel?> GetClimateSettingsAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves the current climate control settings configured on the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ClimateSettingsResponseModel` with climate settings

**Example:**
```csharp
var settings = await client.GetClimateSettingsAsync(vin);
Console.WriteLine($"Temperature: {settings?.Data?.Temperature}°C");
```

### GetClimateStatusAsync
```csharp
Task<ClimateStatusResponseModel?> GetClimateStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves the current climate control status (running/stopped, cabin temperature).

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ClimateStatusResponseModel` with current status

**Example:**
```csharp
var status = await client.GetClimateStatusAsync(vin);
Console.WriteLine($"Climate Running: {status?.Data?.IsRunning}");
Console.WriteLine($"Cabin Temp: {status?.Data?.CabinTemperature}°C");
```

### StartClimateControlAsync
```csharp
Task<ClimateControlResponseModel?> StartClimateControlAsync(string vin, CancellationToken cancellationToken = default)
```
Sends a command to start climate control on the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ClimateControlResponseModel` with operation result

**Throws:** `OperationNotSupportedException` if vehicle doesn't support this

**Example:**
```csharp
var result = await client.StartClimateControlAsync(vin);
if (result?.IsSuccess == true)
    Console.WriteLine("Climate control started");
```

### StopClimateControlAsync
```csharp
Task<ClimateControlResponseModel?> StopClimateControlAsync(string vin, CancellationToken cancellationToken = default)
```
Sends a command to stop climate control on the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ClimateControlResponseModel` with operation result

**Example:**
```csharp
var result = await client.StopClimateControlAsync(vin);
```

### RefreshClimateStatusAsync
```csharp
Task<ClimateControlResponseModel?> RefreshClimateStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Triggers a fresh climate status request from the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ClimateControlResponseModel` with updated status

**Example:**
```csharp
var status = await client.RefreshClimateStatusAsync(vin);
```

---

## Remote Commands

### SendRemoteCommandAsync
```csharp
Task<RemoteCommandResponseModel?> SendRemoteCommandAsync(string vin, RemoteCommandType command, CancellationToken cancellationToken = default)
```
Sends a remote command to the vehicle (lock, unlock, engine start/stop, etc.).

**Parameters:**
- `vin` - Vehicle Identification Number
- `command` - The command to execute (see RemoteCommandType enum)

**Returns:** `RemoteCommandResponseModel` with operation result

**Supported Commands:**
- `Lock` - Lock the vehicle
- `Unlock` - Unlock the vehicle
- `EngineStart` - Start the engine
- `EngineStop` - Stop the engine
- `HazardLights` - Toggle hazard lights
- `Headlights` - Control headlights
- `Trunk` - Open/close trunk

**Example:**
```csharp
var result = await client.SendRemoteCommandAsync(vin, RemoteCommandType.Lock);
if (result?.IsSuccess == true)
    Console.WriteLine("Vehicle locked");
```

**Note:** Check `RemoteServiceCapabilities` on the vehicle to verify support before calling.

### GetRemoteStatusAsync
```csharp
Task<RemoteStatusResponseModel?> GetRemoteStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves the current remote service status for the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `RemoteStatusResponseModel` with remote service status

**Example:**
```csharp
var status = await client.GetRemoteStatusAsync(vin);
```

---

## Vehicle Status & Diagnostics

### GetHealthStatusAsync
```csharp
Task<HealthStatusResponseModel?> GetHealthStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves vehicle health diagnostics information.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `HealthStatusResponseModel` with health status

**Example:**
```csharp
var health = await client.GetHealthStatusAsync(vin);
```

### GetTelemetryStatusAsync
```csharp
Task<TelemetryStatusResponseModel?> GetTelemetryStatusAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves vehicle telemetry data.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `TelemetryStatusResponseModel` with telemetry data

**Example:**
```csharp
var telemetry = await client.GetTelemetryStatusAsync(vin);
```

---

## Trip & Service History

### GetTripsAsync
```csharp
Task<TripsResponseModel?> GetTripsAsync(string vin, DateOnly from, DateOnly to, bool route = false, bool summary = true, int limit = 50, int offset = 0, CancellationToken cancellationToken = default)
```
Retrieves trip history for the vehicle within a date range.

**Parameters:**
- `vin` - Vehicle Identification Number
- `from` - Start date for trip history
- `to` - End date for trip history
- `route` - Include detailed route data (default: false)
- `summary` - Include trip summary (default: true)
- `limit` - Maximum number of trips to return (default: 50)
- `offset` - Pagination offset (default: 0)

**Returns:** `TripsResponseModel` with trip data

**Example:**
```csharp
var trips = await client.GetTripsAsync(
    vin,
    from: DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
    to: DateOnly.FromDateTime(DateTime.Now),
    route: true,
    limit: 100
);

foreach (var trip in trips?.Data ?? [])
{
    Console.WriteLine($"Trip: {trip.StartTime} - {trip.Distance}km");
}
```

### GetServiceHistoryAsync
```csharp
Task<ServiceHistoryResponseModel?> GetServiceHistoryAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves service and maintenance records for the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `ServiceHistoryResponseModel` with service records

**Example:**
```csharp
var history = await client.GetServiceHistoryAsync(vin);
```

---

## Notifications & Statistics

### GetNotificationsAsync
```csharp
Task<NotificationsResponseModel?> GetNotificationsAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves recent notifications for the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `NotificationsResponseModel` with notifications

**Example:**
```csharp
var notifications = await client.GetNotificationsAsync(vin);
```

### GetDrivingStatisticsAsync
```csharp
Task<DrivingStatisticsResponseModel?> GetDrivingStatisticsAsync(string vin, CancellationToken cancellationToken = default)
```
Retrieves driving statistics and eco-scores for the vehicle.

**Parameters:**
- `vin` - Vehicle Identification Number

**Returns:** `DrivingStatisticsResponseModel` with statistics

**Example:**
```csharp
var stats = await client.GetDrivingStatisticsAsync(vin);
Console.WriteLine($"Eco Score: {stats?.Data?.EcoScore}");
Console.WriteLine($"Driving Events: {stats?.Data?.DrivingEvents}");
```

---

## Exceptions

The library throws specific exceptions for different scenarios:

- **`AuthenticationException`** - Login or token validation failed
- **`ApiException`** - API returned an error or unexpected response
- **`OperationCanceledException`** - Operation was cancelled via CancellationToken
- **`TimeoutException`** - Operation exceeded configured timeout
- **`OperationNotSupportedException`** - Vehicle doesn't support the requested operation
- **`ArgumentException`** - Invalid arguments provided
- **`ArgumentNullException`** - Required argument is null

---

## Response Models

All API methods return model classes or null on error. Response models follow this pattern:

```csharp
public class ResponseModel<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}
```

Always check for null or `IsSuccess` before accessing `Data`.
