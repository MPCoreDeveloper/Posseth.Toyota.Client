# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2025-06-29

### Added

- **Trips API** — `GetTripsAsync()` to retrieve trip history with date range, route, and summary options
- **Climate Control** — `GetClimateSettingsAsync()`, `GetClimateStatusAsync()`, `RefreshClimateStatusAsync()`, `StartClimateControlAsync()`, `StopClimateControlAsync()`
- **Remote Commands** — `SendRemoteCommandAsync()` with `RemoteCommandType` enum supporting door lock/unlock, engine start/stop, hazard lights, headlights, and trunk
- **Vehicle Association** — `GetVehicleAssociationAsync()` for authenticated user's vehicle associations
- **Driving Statistics** — `GetDrivingStatisticsAsync()` for eco-scores, fuel consumption, and driving metrics
- **Lock Status** — `GetLockStatusAsync()` for door, trunk, and window lock states
- Response models for all new endpoints
- `SendRequestWithBodyAsync` helper for POST requests with JSON payloads
- Integration tests for all new API methods
- Unit test for `RemoteCommandType` enum values
- OSS directory structure with `docs/`, `CONTRIBUTING.md`, `SECURITY.md`, `CHANGELOG.md`
- GitHub issue and PR templates

### Changed

- Upgraded test project to xUnit v3 (removed xUnit v2 workarounds)
- Removed unused `Microsoft.CodeAnalysis.*` testing packages from test project

## [1.0.0] - 2025-06-28

### Added

- Initial release
- Toyota Connected Services authentication (OpenID Connect flow with ForgeRock AM)
- Token caching with automatic refresh
- Fluent client configuration (`UseCredentials`, `UseLogger`, `UseTimeout`, `UseTokenCaching`)
- **Vehicles** — `GetVehiclesAsync()` to list all vehicles on the account
- **Electric Status** — `GetElectricAsync()` for battery level, charging status, EV range
- **Electric Realtime** — `GetElectricRealtimeStatusAsync()` to trigger live status from vehicle
- **Location** — `GetLocationAsync()` for GPS coordinates and display name
- **Health Status** — `GetHealthStatusAsync()` for engine oil and warning indicators
- **Telemetry** — `GetTelemetryStatusAsync()` for odometer and distance-to-empty
- **Notifications** — `GetNotificationsAsync()` for notification history
- **Remote Status** — `GetRemoteStatusAsync()` for combined remote vehicle status
- **Service History** — `GetServiceHistoryAsync()` for maintenance records
- Comprehensive response models for all Toyota API endpoints
- Console demo application with interactive credential input
- Unit and integration test suite

[Unreleased]: https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/releases/tag/v1.0.0
