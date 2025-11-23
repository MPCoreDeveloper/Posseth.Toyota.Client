using Posseth.Toyota.Client.Const;
using Posseth.Toyota.Client.Interfaces;
using Posseth.Toyota.Client.Services;
using Xunit;

namespace Posseth.Toyota.Client.Tests
{
    public class UnitTest1
    {
        // To run these tests, you need to set the environment variables TOYOTA_USERNAME and TOYOTA_PASSWORD
        // with your Toyota Connected Services credentials.
        // In PowerShell, run:
        // $env:TOYOTA_USERNAME = "your_username_here"
        // $env:TOYOTA_PASSWORD = "your_password_here"
        // Then execute: dotnet test
        // Note: These variables are not persisted; set them in each session or use a script.

        private const string UsernameEnv = "TOYOTA_USERNAME";
        private const string PasswordEnv = "TOYOTA_PASSWORD";

        private static IMyToyotaClient CreateClient()
        {
            var username = Environment.GetEnvironmentVariable(UsernameEnv);
            var password = Environment.GetEnvironmentVariable(PasswordEnv);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Username and password must be set in environment variables.");
            }

            return new MyToyotaClient()
                .UseCredentials(username, password)
                .UseTimeout(30);
        }

        [Fact]
        public void ClientVersion_IsCorrect()
        {
            Assert.Equal("2.14.0", Constants.CLIENT_VERSION);
        }

        [Fact]
        public void KilometersUnit_IsCorrect()
        {
            Assert.Equal("km", Constants.KILOMETERS_UNIT);
        }

        [Fact]
        public void MilesUnit_IsCorrect()
        {
            Assert.Equal("mi", Constants.MILES_UNIT);
        }

        [Fact]
        public void LToMpgFactor_IsCorrect()
        {
            Assert.Equal(235.215, Constants.L_TO_MPG_FACTOR);
        }

        [Fact]
        public void MlToLFactor_IsCorrect()
        {
            Assert.Equal(1000.0, Constants.ML_TO_L_FACTOR);
        }

        [Fact]
        public void MlToGalFactor_IsCorrect()
        {
            Assert.Equal(3785.0, Constants.ML_TO_GAL_FACTOR);
        }

        [Fact]
        public void KmToMilesFactor_IsCorrect()
        {
            Assert.Equal(0.621371192, Constants.KM_TO_MILES_FACTOR);
        }

        [Fact]
        public void MilesToKmFactor_IsCorrect()
        {
            Assert.Equal(1.60934, Constants.MILES_TO_KM_FACTOR);
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed()
        {
            var client = CreateClient();
            var result = await client.LoginAsync();
            Assert.True(result);
        }

        [Fact]
        public async Task GetVehiclesAsync_ShouldReturnVehicles()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
        }

        [Fact]
        public async Task GetElectricAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetElectricAsync(vin);
            Assert.NotNull(result);
            // Add more assertions based on expected data
        }

        [Fact]
        public async Task GetElectricRealtimeStatusAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetElectricRealtimeStatusAsync(vin);
            Assert.NotNull(result);
            // Wait for the car to be contacted, as in the console app
            await Task.Delay(2 * 60 * 1000); // Wait 2 minutes
        }

        [Fact]
        public async Task GetLocationAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetLocationAsync(vin);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetHealthStatusAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetHealthStatusAsync(vin);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTelemetryStatusAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetTelemetryStatusAsync(vin);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNotificationsAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetNotificationsAsync(vin);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetRemoteStatusAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetRemoteStatusAsync(vin);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetServiceHistoryAsync_ShouldReturnData()
        {
            var client = CreateClient();
            await client.LoginAsync();
            var vehicles = await client.GetVehiclesAsync();
            Assert.NotNull(vehicles);
            Assert.NotNull(vehicles.Payload);
            Assert.True(vehicles.Payload.Any());
            var vehicle = vehicles.Payload[0];
            Assert.NotNull(vehicle);
            var vin = vehicle.Vin;
            Assert.NotNull(vin);
            var result = await client.GetServiceHistoryAsync(vin);
            Assert.NotNull(result);
        }
    }
}
