# Getting Started

## Installation

Install the NuGet package:

```bash
dotnet add package Posseth.Toyota.Client
```

Or via NuGet Package Manager:

```
Install-Package Posseth.Toyota.Client
```

## Quick Start

### 1. Create a Client

```csharp
using Posseth.Toyota.Client;
using Posseth.Toyota.Client.Models;

var client = new MyToyotaClient()
    .UseCredentials("your-username", "your-password")
    .UseTimeout(30)
    .UseTokenCaching(true);
```

### 2. Authenticate

```csharp
var loginSuccess = await client.LoginAsync();
if (!loginSuccess)
{
    Console.WriteLine("Login failed");
    return;
}
```

### 3. Get Vehicles

```csharp
var vehicles = await client.GetVehiclesAsync();
if (vehicles?.Data != null)
{
    foreach (var vehicle in vehicles.Data)
    {
        Console.WriteLine($"VIN: {vehicle.Vin}, Name: {vehicle.Nickname}");
    }
}
```

### 4. Get Vehicle Status

```csharp
var vin = "JTHJP5C27D5012345"; // Your vehicle VIN
var location = await client.GetLocationAsync(vin);
Console.WriteLine($"Location: {location?.Data?.Latitude}, {location?.Data?.Longitude}");

var electric = await client.GetElectricAsync(vin);
Console.WriteLine($"Battery Level: {electric?.Data?.BatteryLevel}%");
```

## Common Tasks

### Start Climate Control

```csharp
var result = await client.StartClimateControlAsync(vin);
if (result?.IsSuccess == true)
{
    Console.WriteLine("Climate control started");
}
```

### Lock Vehicle

```csharp
var result = await client.SendRemoteCommandAsync(vin, RemoteCommandType.Lock);
if (result?.IsSuccess == true)
{
    Console.WriteLine("Vehicle locked");
}
```

### Get Trip History

```csharp
var trips = await client.GetTripsAsync(
    vin,
    from: DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
    to: DateOnly.FromDateTime(DateTime.Now),
    route: true,      // Include route data
    summary: true,    // Include summary
    limit: 50
);

foreach (var trip in trips?.Data ?? [])
{
    Console.WriteLine($"Trip: {trip.StartTime} - Distance: {trip.Distance}km");
}
```

### Refresh Climate Status

```csharp
var status = await client.RefreshClimateStatusAsync(vin);
Console.WriteLine($"Climate Status: {status?.Data?.Status}");
```

## Configuration Options

### Timeout

Set request timeout (in seconds):

```csharp
client.UseTimeout(60);  // 60 second timeout
```

### Logging

Enable logging for debugging:

```csharp
client.UseLogger(message => Console.WriteLine(message));

// Or with structured logging
client.UseLogger(message => logger.LogInformation(message));
```

### Token Caching

Configure token caching:

```csharp
// Enable caching (default: true)
client.UseTokenCaching(true);

// Set custom cache file location
client.UseTokenCacheFilename("my_tokens.json");

// Disable caching (tokens must be re-acquired each session)
client.UseTokenCaching(false);
```

## Cancellation

All async methods support cancellation:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var vehicles = await client.GetVehiclesAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

## Error Handling

```csharp
try
{
    var result = await client.GetElectricAsync(vin);
}
catch (AuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (ApiException ex)
{
    Console.WriteLine($"API error: {ex.Message}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
catch (TimeoutException)
{
    Console.WriteLine("Request timeout");
}
```

## Environment Variables

You can use environment variables for sensitive data:

```csharp
var username = Environment.GetEnvironmentVariable("TOYOTA_USERNAME")
    ?? throw new InvalidOperationException("TOYOTA_USERNAME not set");
var password = Environment.GetEnvironmentVariable("TOYOTA_PASSWORD")
    ?? throw new InvalidOperationException("TOYOTA_PASSWORD not set");

var client = new MyToyotaClient()
    .UseCredentials(username, password);
```

## Dependency Injection (ASP.NET Core)

```csharp
services.AddSingleton<IMyToyotaClient>(serviceProvider =>
    new MyToyotaClient()
        .UseCredentials(
            configuration["Toyota:Username"],
            configuration["Toyota:Password"]
        )
        .UseTimeout(int.Parse(configuration["Toyota:TimeoutSeconds"]))
        .UseLogger(serviceProvider.GetRequiredService<ILogger<MyToyotaClient>>().LogInformation)
);
```

## Examples

See the `samples/` directory for complete working examples.

## Next Steps

- Check [API Documentation](./API.md) for detailed method reference
- Review [Architecture](./ARCHITECTURE.md) for design details
- Look at the [samples](../samples/) for real-world usage
- Report issues and request features on [GitHub Issues](https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/issues)
