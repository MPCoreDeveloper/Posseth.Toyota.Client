<div align="center">
  <img src="../assets/ToyoCL.png" alt="Posseth Toyota Client logo" width="90" height="90" />
</div>

# Getting Started

## Table of Contents

- [Installation](#installation)
- [Standalone Usage (no DI)](#standalone-usage-no-di)
- [Dependency Injection](#dependency-injection)
  - [ASP.NET Core / Generic Host](#aspnet-core--generic-host)
  - [Option A — Inline lambda](#option-a--inline-lambda)
  - [Option B — appsettings.json binding](#option-b--appsettingsjson-binding)
  - [Injecting the client](#injecting-the-client)
  - [Credentials management](#credentials-management)
- [Common Tasks](#common-tasks)
- [Cancellation](#cancellation)
- [Error Handling](#error-handling)
- [Environment Variables](#environment-variables)

---

## Installation

Install the NuGet package:

```bash
dotnet add package Posseth.Toyota.Client
```

Or via NuGet Package Manager:

```
Install-Package Posseth.Toyota.Client
```

---

## Standalone Usage (no DI)

If you are building a console app or any project that does not use a DI container,
you can create the client directly using the fluent builder API:

```csharp
using Posseth.Toyota.Client.Services;
using Posseth.Toyota.Client.Interfaces;

IMyToyotaClient client = new MyToyotaClient()
    .UseCredentials("your@email.com", "your-password")
    .UseTimeout(30)
    .UseTokenCaching(true)
    .UseLogger(msg => Console.WriteLine(msg));  // optional

var loginSuccess = await client.LoginAsync();
if (!loginSuccess)
    throw new InvalidOperationException("Login failed");
```

### Get Vehicles

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

### Get Vehicle Status

```csharp
var vin = "JTHJP5C27D5012345";
var location = await client.GetLocationAsync(vin);
Console.WriteLine($"Location: {location?.Data?.Latitude}, {location?.Data?.Longitude}");

var electric = await client.GetElectricAsync(vin);
Console.WriteLine($"Battery Level: {electric?.Data?.BatteryLevel}%");
```

---

## Dependency Injection

`Posseth.Toyota.Client` ships with first-class support for the
[.NET Options pattern](https://learn.microsoft.com/dotnet/core/extensions/options)
and `Microsoft.Extensions.DependencyInjection`.

The library exposes `IMyToyotaClient` as the public abstraction and provides
`AddToyotaClient()` extension methods on `IServiceCollection`.

### ASP.NET Core / Generic Host

Add the using directive once in `Program.cs`:

```csharp
using Posseth.Toyota.Client.Extensions;
```

---

### Option A — Inline lambda

Use the lambda overload when you want to configure the client directly in code,
e.g. reading from environment variables or a secrets manager:

```csharp
// Program.cs
builder.Services.AddToyotaClient(options =>
{
    options.Username       = builder.Configuration["Toyota:Username"]!;
    options.Password       = builder.Configuration["Toyota:Password"]!;
    options.TimeoutSeconds = 30;
    options.UseTokenCaching = true;
    options.Logger         = msg => Console.WriteLine(msg); // optional
});
```

---

### Option B — `appsettings.json` binding

Add a dedicated section to your `appsettings.json`:

```json
{
  "ToyotaClient": {
    "Username": "your@email.com",
    "Password": "your-password",
    "TimeoutSeconds": 30,
    "UseTokenCaching": true,
    "TokenCacheFilename": "toyota_token_cache.json"
  }
}
```

Then pass the section to `AddToyotaClient`:

```csharp
// Program.cs
using Posseth.Toyota.Client;
using Posseth.Toyota.Client.Extensions;

builder.Services.AddToyotaClient(
    builder.Configuration.GetSection(ToyotaClientOptions.SectionName));
```

> **`ToyotaClientOptions.SectionName`** is the string constant `"ToyotaClient"`.

---

### Injecting the client

Once registered, inject `IMyToyotaClient` into any service, controller, or minimal-API handler:

#### Constructor injection (recommended)

```csharp
using Posseth.Toyota.Client.Interfaces;

public class VehicleService(IMyToyotaClient client)
{
    public async Task<string?> GetFirstVinAsync(CancellationToken ct = default)
    {
        await client.LoginAsync(ct);
        var vehicles = await client.GetVehiclesAsync(ct);
        return vehicles?.Data?.FirstOrDefault()?.Vin;
    }
}
```

Register the service and wire it up:

```csharp
builder.Services.AddScoped<VehicleService>();
```

#### Minimal API

```csharp
app.MapGet("/vehicles", async (IMyToyotaClient client, CancellationToken ct) =>
{
    await client.LoginAsync(ct);
    return await client.GetVehiclesAsync(ct);
});
```

#### `IOptions<ToyotaClientOptions>` — reading options directly

If you need to read the configured options elsewhere:

```csharp
using Microsoft.Extensions.Options;
using Posseth.Toyota.Client;

public class MyDiagnosticsService(IOptions<ToyotaClientOptions> options)
{
    public string GetConfiguredUsername() => options.Value.Username;
}
```

---

### Credentials management

| Environment | Recommended approach |
|---|---|
| Local development | [.NET Secret Manager](https://learn.microsoft.com/aspnet/core/security/app-secrets) |
| CI / staging | Environment variables |
| Production | [Azure Key Vault](https://learn.microsoft.com/azure/key-vault) / secrets provider |

**Never** commit credentials to source control.  
The token cache file (`toyota_credentials_cache_contains_secrets.json` by default)
should also be added to `.gitignore`.

---

## Common Tasks

### Start Climate Control

```csharp
var result = await client.StartClimateControlAsync(vin);
if (result?.IsSuccess == true)
    Console.WriteLine("Climate control started");
```

### Lock Vehicle

```csharp
var result = await client.SendRemoteCommandAsync(vin, RemoteCommandType.Lock);
if (result?.IsSuccess == true)
    Console.WriteLine("Vehicle locked");
```

### Get Trip History

```csharp
var trips = await client.GetTripsAsync(
    vin,
    from: DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
    to: DateOnly.FromDateTime(DateTime.Now),
    route: true,
    summary: true,
    limit: 50
);

foreach (var trip in trips?.Data ?? [])
    Console.WriteLine($"Trip: {trip.StartTime} - Distance: {trip.Distance}km");
```

### Refresh Climate Status

```csharp
var status = await client.RefreshClimateStatusAsync(vin);
Console.WriteLine($"Climate Status: {status?.Data?.Status}");
```

---

## Configuration Options

| Property | Type | Default | Description |
|---|---|---|---|
| `Username` | `string` | — | MyToyota account e-mail |
| `Password` | `string` | — | MyToyota account password |
| `TimeoutSeconds` | `int` | `60` | HTTP request timeout |
| `UseTokenCaching` | `bool` | `true` | Cache the bearer token to disk |
| `TokenCacheFilename` | `string` | `toyota_credentials_cache_contains_secrets.json` | Token cache file path |
| `Logger` | `Action<string>?` | `null` | Optional diagnostic logger |

### Timeout

```csharp
options.TimeoutSeconds = 60;
```

### Logging

```csharp
// Console
options.Logger = message => Console.WriteLine(message);

// Bridge to Microsoft.Extensions.Logging
options.Logger = message => logger.LogDebug("{Message}", message);
```

### Token Caching

```csharp
options.UseTokenCaching     = true;
options.TokenCacheFilename  = "my_tokens.json";
```

---

## Cancellation

All async methods accept a `CancellationToken`:

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

---

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

---

## Environment Variables

You can read credentials from environment variables to avoid hardcoding them:

```csharp
// Standalone
var client = new MyToyotaClient()
    .UseCredentials(
        Environment.GetEnvironmentVariable("TOYOTA_USERNAME")
            ?? throw new InvalidOperationException("TOYOTA_USERNAME not set"),
        Environment.GetEnvironmentVariable("TOYOTA_PASSWORD")
            ?? throw new InvalidOperationException("TOYOTA_PASSWORD not set"));

// DI / lambda
builder.Services.AddToyotaClient(options =>
{
    options.Username = Environment.GetEnvironmentVariable("TOYOTA_USERNAME")
        ?? throw new InvalidOperationException("TOYOTA_USERNAME not set");
    options.Password = Environment.GetEnvironmentVariable("TOYOTA_PASSWORD")
        ?? throw new InvalidOperationException("TOYOTA_PASSWORD not set");
});
