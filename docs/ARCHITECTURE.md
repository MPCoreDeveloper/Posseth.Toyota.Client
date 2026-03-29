# Architecture Overview

## Project Structure

The Posseth.Toyota.Client is a .NET 10 C# client library for interacting with Toyota's MyToyota API.

```
Posseth.Toyota.Client/
├── src/
│   └── Posseth.Toyota.Client/
│       ├── Interfaces/              # Public API contracts
│       ├── Models/                  # API response/request models
│       ├── Services/                # Core business logic
│       ├── Exceptions/              # Custom exception types
│       ├── Configuration/           # Configuration classes
│       └── MyToyotaClient.cs        # Main public API entry point
├── tests/
│   └── Posseth.Toyota.Client.Tests/ # Unit and integration tests
├── samples/
│   └── Posseth.Toyota.Demo.ConsoleApp/ # Example usage
└── docs/                            # Documentation
```

## Core Components

### IMyToyotaClient
The main interface and entry point for all client operations.

**Key Features:**
- Fluent configuration API (`UseCredentials`, `UseLogger`, `UseTimeout`, etc.)
- Async operations for all API calls
- Full cancellation token support
- Token caching support

### Models
Contains all request/response models for the MyToyota API:
- `VehiclesModel` - Vehicle information
- `ElectricResponseModel` - EV battery status
- `LocationResponseModel` - Vehicle location
- `ClimateControlResponseModel` - Climate control operations
- `RemoteCommandResponseModel` - Remote vehicle commands
- And many more...

### Services
Internal services handling:
- HTTP communication with the MyToyota API
- Authentication and token management
- Request/response serialization
- Error handling and retry logic

## Design Principles

1. **Async-First**: All I/O operations are async with proper `CancellationToken` support
2. **Fluent Configuration**: Builder pattern for easy client configuration
3. **Type-Safe**: Strong typing for all API contracts
4. **Error Handling**: Custom exceptions for different failure scenarios
5. **Extensibility**: Dependency injection friendly architecture

## Authentication Flow

```
1. Create client with credentials: client.UseCredentials(username, password)
2. Call LoginAsync() to authenticate
3. Tokens are cached (configurable) for subsequent API calls
4. All subsequent API calls use the cached token
```

## API Categories

### Vehicle Information
- `GetVehiclesAsync()` - List all associated vehicles
- `GetVehicleAssociationAsync()` - Get association details

### Electric/EV Status
- `GetElectricAsync()` - EV battery and charging info
- `GetElectricRealtimeStatusAsync()` - Real-time EV status

### Climate Control
- `GetClimateSettingsAsync()` - Current climate settings
- `GetClimateStatusAsync()` - Current climate control status
- `StartClimateControlAsync()` - Start climate control
- `StopClimateControlAsync()` - Stop climate control
- `RefreshClimateStatusAsync()` - Refresh climate status

### Remote Commands
- `SendRemoteCommandAsync()` - Lock, unlock, start engine, etc.

### Vehicle Status
- `GetLocationAsync()` - Vehicle GPS location
- `GetLockStatusAsync()` - Door/trunk lock status
- `GetHealthStatusAsync()` - Vehicle health diagnostics
- `GetTelemetryStatusAsync()` - Telemetry data

### Trip & Service History
- `GetTripsAsync()` - Trip history with optional route data
- `GetServiceHistoryAsync()` - Service and maintenance records

### Other
- `GetNotificationsAsync()` - Recent notifications
- `GetRemoteStatusAsync()` - Remote service status
- `GetDrivingStatisticsAsync()` - Eco-scores and statistics

## Configuration Options

```csharp
var client = new MyToyotaClient()
    .UseCredentials("username", "password")
    .UseTimeout(30)                              // API timeout in seconds
    .UseTokenCaching(true)                       // Cache tokens for reuse
    .UseTokenCacheFilename("tokens.json")        // Custom cache location
    .UseLogger(msg => Console.WriteLine(msg));   // Custom logging
```

## Thread Safety

The client is designed to be used as a singleton or long-lived instance. It is thread-safe for concurrent API calls.

## Token Caching

By default, tokens are cached to a file to avoid unnecessary login calls. Configure with:
- `UseTokenCaching(bool)` - Enable/disable token caching
- `UseTokenCacheFilename(string)` - Set custom cache file location

## Error Handling

The library throws custom exceptions for different scenarios:
- `AuthenticationException` - Login or token validation failed
- `ApiException` - API returned an error
- `OperationCanceledException` - Operation was cancelled
- `TimeoutException` - Operation exceeded timeout

## Performance Considerations

- Token caching reduces login overhead
- Batch operations where possible
- Use appropriate timeout values for your use case
- Cancel operations that are no longer needed
