using Posseth.Toyota.Client.Interfaces;
using Posseth.Toyota.Client.Models;
using Posseth.Toyota.Client.Services;

namespace Posseth.Toyota.Demo.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Toyota Connected Services Demo");
            Console.WriteLine("==============================");

            string username = string.Empty, password = string.Empty;

            // Try to load credentials from file, or ask the user
            if (File.Exists(@"C:\Credentials\ToyotaClientUsername.txt") &&
                File.Exists(@"C:\Credentials\ToyotaClientPassword.txt"))
            {
                username = await File.ReadAllTextAsync(@"C:\Credentials\ToyotaClientUsername.txt");
                password = await File.ReadAllTextAsync(@"C:\Credentials\ToyotaClientPassword.txt");
                Console.WriteLine("Credentials loaded from secure files.");
            }
            else
            {
                Console.Write("Enter your Toyota username: ");
                username = Console.ReadLine() ?? string.Empty;

                Console.Write("Enter your Toyota password: ");
                password = ReadPassword();
                Console.WriteLine();
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Invalid credentials. Application will exit.");
                return;
            }

            // Initialize the client with modern fluent interface
            // Using ToyotaRestClient instead of MyToyotaClient
            IMyToyotaClient client = new MyToyotaClient()
                .UseCredentials(username, password)
                .UseLogger(message => Console.WriteLine($"[Client] {message}"))
                .UseTimeout(30)
                .UseTokenCaching(true);

            Console.WriteLine("Logging in to Toyota Connected Services API...");

            try

            {
                bool loginSuccess = await client.LoginAsync();
                if (!loginSuccess)
                {
                    Console.WriteLine("Login failed. Check your credentials.");
                    return;
                }

                Console.WriteLine("Login successful!\n");

                // Retrieve vehicles
                var vehicles = await client.GetVehiclesAsync();

                if (vehicles?.Payload == null || !vehicles.Payload.Any())
                {
                    Console.WriteLine("No vehicles found for this account.");
                    return;
                }

                // For each vehicle, retrieve all information
                foreach (var vehicle in vehicles.Payload)
                {
                    if (vehicle != null)
                    {
                        await DisplayVehicleInfoAsync(client, vehicle);
                    }
                    else
                    {
                        Console.WriteLine("Skipping null vehicle entry.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task DisplayVehicleInfoAsync(IMyToyotaClient client, Vehicle vehicle)
        {
            if (vehicle == null)
            {
                Console.WriteLine("Cannot display information for null vehicle.");
                return;
            }

            if (string.IsNullOrWhiteSpace(vehicle.Vin))
            {
                Console.WriteLine("Cannot display information for vehicle with no VIN.");
                return;
            }

            string vin = vehicle.Vin;  // Safely store the non-null VIN for use in API calls

            Console.WriteLine($"==================== Vehicle {vin} ====================");
            Console.WriteLine($"Model              : {vehicle.CarlineName}");
            Console.WriteLine($"Color              : {vehicle.Color}");
            Console.WriteLine($"Chassis number     : {vin}");
            Console.WriteLine($"IMEI               : {vehicle.Imei}");
            Console.WriteLine();

            // Request realtime status
            Console.WriteLine("Requesting realtime status...");
            try
            {
                var realtimeStatus = await client.GetElectricRealtimeStatusAsync(vin);
                if (realtimeStatus?.status != null)
                {
                    Console.WriteLine("Electric realtime status:");
                    if (realtimeStatus.status.messages != null)
                    {
                        foreach (var message in realtimeStatus.status.messages)
                        {
                            Console.WriteLine(message?.detailedDescription);
                        }
                    }
                    Console.WriteLine($"AppRequestNo: {realtimeStatus.payload?.appRequestNo}");

                    // For an electric car we need to wait until the car has been contacted
                    Console.WriteLine("We need to wait for the Toyota server to contact the car.");
                    await WaitWithProgressAsync(2 * 60); // Wait for 2 minutes
                }
                else
                {
                    Console.WriteLine("No realtime status data available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving realtime status: {ex.Message}");
            }

            // Request electric info
            try
            {
                var electric = await client.GetElectricAsync(vin);
                if (electric?.payload != null)
                {
                    Console.WriteLine("Electric info:");
                    Console.WriteLine($"Charging status    : {electric.payload.chargingStatus}");
                    Console.WriteLine($"Battery level      : {electric.payload.batteryLevel} %");
                    Console.WriteLine($"Range              : {electric.payload.evRange?.value:N0} {electric.payload.evRange?.unit}");
                    Console.WriteLine($"Range with AC      : {electric.payload.evRangeWithAc?.value:N0} {electric.payload.evRangeWithAc?.unit}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No electric info available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving electric info: {ex.Message}");
            }

            // Request location info
            try
            {
                var location = await client.GetLocationAsync(vin);
                if (location?.payload?.vehicleLocation != null)
                {
                    Console.WriteLine("Location info:");
                    Console.WriteLine($"Status             : {location.status}");
                    Console.WriteLine($"Code               : {location.code}");
                    Console.WriteLine($"Location           : {location.payload.vehicleLocation.latitude} / {location.payload.vehicleLocation.longitude}");
                    Console.WriteLine($"Display name       : {location.payload.vehicleLocation.displayName}");
                    Console.WriteLine($"Acquisition time   : {location.payload.vehicleLocation.locationAcquisitionDatetime}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No location info available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving location info: {ex.Message}");
            }

            // Request health status
            try
            {
                var health = await client.GetHealthStatusAsync(vin);
                if (health?.payload != null)
                {
                    Console.WriteLine("Health status:");
                    Console.WriteLine($"Oil data           : {health.payload.quantityOfEngOilIcon?.Count ?? 0} entries");
                    Console.WriteLine($"Warnings           : {health.payload.warning?.Count ?? 0} entries");
                    Console.WriteLine($"Last update        : {health.payload.wnglastUpdTime}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No health status available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving health status: {ex.Message}");
            }

            // Request telemetry status
            try
            {
                var telemetry = await client.GetTelemetryStatusAsync(vin);
                if (telemetry?.payload != null)
                {
                    Console.WriteLine("Telemetry status:");
                    Console.WriteLine($"Distance to empty  : {telemetry.payload.distanceToEmpty?.value} {telemetry.payload.distanceToEmpty?.unit}");
                    Console.WriteLine($"Odometer           : {telemetry.payload.odometer?.value} {telemetry.payload.odometer?.unit}");
                    Console.WriteLine($"Timestamp          : {telemetry.payload.timestamp}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No telemetry status available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving telemetry status: {ex.Message}");
            }

            // Request remote status
            try
            {
                var remote = await client.GetRemoteStatusAsync(vin);
                if (remote?.payload != null)
                {
                    Console.WriteLine("Remote status:");
                    Console.WriteLine($"Position           : {remote.payload.latitude} / {remote.payload.longitude}");
                    Console.WriteLine($"Position time      : {remote.payload.locationAcquisitionDatetime}");
                    Console.WriteLine($"Occurrence date    : {remote.payload.occurrenceDate}");

                    if (remote.payload.telemetry != null)
                    {
                        Console.WriteLine($"Fuel age           : {remote.payload.telemetry.fugage?.value} {remote.payload.telemetry.fugage?.unit}");
                        Console.WriteLine($"Odo                : {remote.payload.telemetry.odo?.value} {remote.payload.telemetry.odo?.unit}");
                        Console.WriteLine($"Range              : {remote.payload.telemetry.rage?.value} {remote.payload.telemetry.rage?.unit}");
                    }

                    if (remote.payload.vehicleStatus != null)
                    {
                        Console.WriteLine("Vehicle status:");
                        foreach (var status in remote.payload.vehicleStatus)
                        {
                            if (status?.sections != null)
                            {
                                foreach (var section in status.sections)
                                {
                                    if (section != null)
                                    {
                                        var sectionName = section.section?.Replace("carstatus_item_", "") ?? "unknown";
                                        var values = section.values?.Select(v => v?.value?.Replace("carstatus_", "") ?? "unknown") ?? Array.Empty<string>();
                                        Console.WriteLine($"    {sectionName,-30}: {string.Join(',', values)}");
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No remote status available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving remote status: {ex.Message}");
            }

            // Request service history
            try
            {
                var service = await client.GetServiceHistoryAsync(vin);
                if (service?.payload?.serviceHistories != null && service.payload.serviceHistories.Any())
                {
                    Console.WriteLine("Service events:");
                    foreach (var ev in service.payload.serviceHistories.OrderBy(h => h.serviceDate))
                    {
                        if (ev != null)
                        {
                            Console.WriteLine($"    {ev.serviceDate}     : {ev.mileage,6} {ev.unit,-2}:   {ev.serviceCategory} - {ev.serviceProvider} (ID {ev.serviceHistoryId})");
                        }
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("No service history available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving service history: {ex.Message}");
            }
        }

        private static async Task WaitWithProgressAsync(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                Console.Write($"\rWaiting {i} seconds...");
                await Task.Delay(1000);
            }
            Console.WriteLine();
        }

        private static string ReadPassword()
        {
            var password = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            return password;
        }
    }
}
