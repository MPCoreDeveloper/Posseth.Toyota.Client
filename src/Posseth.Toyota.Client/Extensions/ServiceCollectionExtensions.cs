using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Posseth.Toyota.Client.Interfaces;
using Posseth.Toyota.Client.Services;

namespace Posseth.Toyota.Client.Extensions;

/// <summary>
/// Extension methods for registering Toyota MyT API client services with an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>
/// Call one of the <c>AddToyotaClient</c> overloads in your application's composition root
/// (e.g. <c>Program.cs</c>) to wire up <see cref="IMyToyotaClient"/> for dependency injection.
/// The client is registered as <b>Transient</b> because it holds per-session authentication state.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Toyota MyT API client using an inline configuration delegate.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configure">
    /// A delegate that configures a <see cref="ToyotaClientOptions"/> instance.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// // Program.cs (ASP.NET Core / Generic Host)
    /// builder.Services.AddToyotaClient(options =>
    /// {
    ///     options.Username      = "your@email.com";
    ///     options.Password      = "your-password";
    ///     options.TimeoutSeconds = 30;
    ///     options.Logger        = Console.WriteLine;
    /// });
    ///
    /// // Inject and use via constructor:
    /// public class VehicleService(IMyToyotaClient client) { ... }
    /// </code>
    /// </example>
    public static IServiceCollection AddToyotaClient(
        this IServiceCollection services,
        Action<ToyotaClientOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<ToyotaClientOptions>()
                .Configure(configure)
                .ValidateOnStart();

        RegisterClient(services);
        return services;
    }

    /// <summary>
    /// Registers the Toyota MyT API client by binding options from an <see cref="IConfiguration"/> section.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">
    /// The configuration section to bind — typically obtained via
    /// <c>builder.Configuration.GetSection(<see cref="ToyotaClientOptions.SectionName"/>)</c>.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // {
    /// //   "ToyotaClient": {
    /// //     "Username": "your@email.com",
    /// //     "Password": "your-password",
    /// //     "TimeoutSeconds": 30
    /// //   }
    /// // }
    ///
    /// // Program.cs:
    /// builder.Services.AddToyotaClient(
    ///     builder.Configuration.GetSection(ToyotaClientOptions.SectionName));
    /// </code>
    /// </example>
    public static IServiceCollection AddToyotaClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<ToyotaClientOptions>()
                .Bind(configuration)
                .ValidateOnStart();

        RegisterClient(services);
        return services;
    }

    // PERF: Shared registration logic — avoids duplication between overloads.
    private static void RegisterClient(IServiceCollection services) =>
        services.AddTransient<IMyToyotaClient>(static sp =>
        {
            var options = sp.GetRequiredService<IOptions<ToyotaClientOptions>>().Value;

            IMyToyotaClient client = new MyToyotaClient()
                .UseCredentials(options.Username, options.Password)
                .UseTimeout(options.TimeoutSeconds)
                .UseTokenCaching(options.UseTokenCaching)
                .UseTokenCacheFilename(options.TokenCacheFilename);

            if (options.Logger is not null)
                client = client.UseLogger(options.Logger);

            return client;
        });
}
