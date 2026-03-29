namespace Posseth.Toyota.Client;

/// <summary>
/// Configuration options for the <c>Posseth.Toyota.Client</c> library.
/// </summary>
/// <remarks>
/// <para>
/// Bind this class from <c>appsettings.json</c> or configure it inline via
/// <see cref="Extensions.ServiceCollectionExtensions.AddToyotaClient(Microsoft.Extensions.DependencyInjection.IServiceCollection, Action{ToyotaClientOptions})"/>.
/// </para>
/// <para>
/// Minimal required configuration:
/// <code>
/// "ToyotaClient": {
///   "Username": "your@email.com",
///   "Password": "your-password"
/// }
/// </code>
/// </para>
/// </remarks>
public sealed class ToyotaClientOptions
{
    /// <summary>
    /// The configuration section name used when binding from <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddToyotaClient(
    ///     builder.Configuration.GetSection(ToyotaClientOptions.SectionName));
    /// </code>
    /// </example>
    public const string SectionName = "ToyotaClient";

    /// <summary>
    /// Gets or sets the MyToyota account username (e-mail address).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MyToyota account password.
    /// </summary>
    /// <remarks>
    /// Store this value securely. Use environment variables, Azure Key Vault, or
    /// the .NET Secret Manager in development. Never hardcode credentials.
    /// </remarks>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP request timeout in seconds.
    /// Defaults to <c>60</c>.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether bearer-token caching is enabled.
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/> (default) the access token is cached to a local file
    /// to avoid unnecessary re-authentication on every application start.
    /// Set to <see langword="false"/> to always perform a fresh login.
    /// </remarks>
    public bool UseTokenCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the filename used to persist the token cache.
    /// Defaults to <c>toyota_credentials_cache_contains_secrets.json</c>.
    /// </summary>
    /// <remarks>
    /// The file is written to the working directory of the application.
    /// Ensure this path is excluded from source control (e.g. added to <c>.gitignore</c>).
    /// </remarks>
    public string TokenCacheFilename { get; set; } =
        "toyota_credentials_cache_contains_secrets.json";

    /// <summary>
    /// Gets or sets an optional delegate that receives diagnostic log messages from the client.
    /// </summary>
    /// <example>
    /// <code>
    /// options.Logger = message => Console.WriteLine(message);
    ///
    /// // Or bridge to Microsoft.Extensions.Logging:
    /// options.Logger = message => logger.LogDebug("{Message}", message);
    /// </code>
    /// </example>
    public Action<string>? Logger { get; set; }
}
