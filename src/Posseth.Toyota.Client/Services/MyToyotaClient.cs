using System.Net; // For CookieContainer
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Posseth.Toyota.Client.Interfaces;
using Posseth.Toyota.Client.Models;

namespace Posseth.Toyota.Client.Services;

public class MyToyotaClient : IMyToyotaClient
{
    // Private fields
    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler _handler;
    private readonly ToyotaApiSettings _settings;
    private readonly CookieContainer _cookieContainer = new(); // New: CookieContainer
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        // CRITICAL FIX: Do NOT ignore null values. The server expects them.
        // This makes System.Text.Json behave like Newtonsoft.Json in the working example.
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };
    private string? _username;
    private string? _password;
    private Action<string>? _logger;
    private int _timeoutInSeconds = 60;
    private TokenCacheItem? _tokenCache;
    private bool _useTokenCaching = true;
    private bool _bypassSslValidation;
    private string _tokenCacheFilename = "toyota_credentials_cache_contains_secrets.json";
    private readonly string _apiKey;
    private bool _warnedAboutMissingApiKey;

    // Constructors
    public MyToyotaClient()
        : this(new ToyotaApiSettings())
    {
    }

    public MyToyotaClient(ToyotaApiSettings settings)
        : this(settings, customHandler: null)
    {
    }

    public MyToyotaClient(HttpClientHandler? customHandler)
        : this(new ToyotaApiSettings(), customHandler)
    {
    }

    public MyToyotaClient(ToyotaApiSettings settings, HttpClientHandler? customHandler = null)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        // SECURITY: The API key is a credential and must not be baked into the library.
        // When it is not supplied through ToyotaApiSettings, fall back to the TOYOTA_API_KEY
        // environment variable so the client works without a hard-coded secret.
        _apiKey = string.IsNullOrWhiteSpace(_settings.ApiKey)
            ? Environment.GetEnvironmentVariable("TOYOTA_API_KEY") ?? string.Empty
            : _settings.ApiKey;

        _handler = customHandler ?? new HttpClientHandler();

        // Configure the handler for this service
        _handler.AllowAutoRedirect = false; // Crucial
        _handler.UseCookies = true; // Crucial
        _handler.CookieContainer = _cookieContainer; // Link the container
        ApplySslValidation();

        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri(_settings.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(_timeoutInSeconds)
        };
    }

    private void ApplySslValidation()
    {
        _handler.ServerCertificateCustomValidationCallback = _bypassSslValidation
            ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            : null;
    }

    // Fluent configuration methods
    public IMyToyotaClient UseCredentials(string username, string password)
    {
        _username = username;
        _password = password;
        return this;
    }

    public IMyToyotaClient UseLogger(Action<string> logger)
    {
        _logger = logger;
        return this;
    }

    public IMyToyotaClient UseTimeout(int timeoutSeconds)
    {
        _timeoutInSeconds = timeoutSeconds;
        _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        return this;
    }

    public IMyToyotaClient UseBypassSslValidation(bool bypassSslValidation)
    {
        _bypassSslValidation = bypassSslValidation;
        ApplySslValidation();
        return this;
    }

    public IMyToyotaClient UseTokenCacheFilename(string tokenCacheFilename)
    {
        _tokenCacheFilename = tokenCacheFilename;
        return this;
    }

    public IMyToyotaClient UseTokenCaching(bool useTokenCaching)
    {
        _useTokenCaching = useTokenCaching;
        return this;
    }

    // Authentication methods
    public async Task<bool> LoginAsync(CancellationToken cancellationToken = default)
    {
        if (_useTokenCaching && File.Exists(_tokenCacheFilename))
        {
            var content = await File.ReadAllTextAsync(_tokenCacheFilename, cancellationToken);
            _tokenCache = JsonSerializer.Deserialize<TokenCacheItem>(content, _jsonOptions);
        }

        if (!IsTokenValid())
            await UpdateTokenAsync(cancellationToken);

        return _tokenCache is { access_token: not null };
    }

    private async Task UpdateTokenAsync(CancellationToken cancellationToken)
    {
        if (_tokenCache?.refresh_token != null)
        {
            try
            {
                await RefreshTokensAsync(cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Failed to refresh token: {ex.Message}");
                // Fallback to full authentication
            }
        }

        await AuthenticateAsync(cancellationToken);
    }

    private async Task AuthenticateAsync(CancellationToken cancellationToken)
    {
        _logger?.Invoke("Authenticating with Toyota servers...");

        AuthenticationModel? data = null;
        AuthenticationModel2? tokenInfo = null;

        // Step 1: Initial Authentication Process
        // This loop mimics the logic from the working RestSharp example.
        // It is stateless and reacts to the callbacks received from the server in each iteration.
        for (int i = 0; i < 10; i++)
        {
            _logger?.Invoke($"Authentication attempt {i + 1}");

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.AuthenticateUrl);

            if (data is null)
            {
                // First request has an empty body
                request.Content = new StringContent("", Encoding.UTF8, "application/json");
                _logger?.Invoke("Sending initial empty content");
            }
            else
            {
                // Subsequent requests send the modified data object back
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                _logger?.Invoke($"Sending data: {json.Length} bytes");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger?.Invoke($"Authentication failed with status code: {response.StatusCode}. Response: {errorContent}");
                throw new InvalidOperationException($"Could not authenticate with Toyota server: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger?.Invoke($"Authentication response {i + 1} (first 200 chars): {content.Substring(0, Math.Min(content.Length, 200))}...");

            try
            {
                // If the response contains the tokenId, we are done with this part.
                if (content.Contains("\"tokenId\""))
                {
                    tokenInfo = JsonSerializer.Deserialize<AuthenticationModel2>(content, _jsonOptions);
                    _logger?.Invoke("Authentication successful, received tokenId");
                    break; // Exit the loop
                }

                // Deserialize the response to handle the callbacks.
                data = JsonSerializer.Deserialize<AuthenticationModel>(content, _jsonOptions);

                if (data?.callbacks is null)
                {
                    _logger?.Invoke("Warning: Response contained no callbacks");
                    continue;
                }

                // Handle all callbacks received in this response.
                foreach (var cb in data.callbacks)
                {
                    if (cb.type == "NameCallback" && cb.output?.FirstOrDefault()?.value?.ToString() == "User Name")
                    {
                        _logger?.Invoke("Found username prompt, providing username");
                        if (cb.input?.Count > 0) cb.input[0].value = _username;
                    }
                    else if (cb.type == "PasswordCallback")
                    {
                        _logger?.Invoke("Found password prompt, providing password");
                        if (cb.input?.Count > 0) cb.input[0].value = _password;
                    }
                    else if (cb.type == "TextOutputCallback" && cb.output?.FirstOrDefault()?.value?.ToString() == "User Not Found")
                    {
                        _logger?.Invoke("Authentication failed: User not found");
                        throw new InvalidOperationException("Authentication Failed: User Not Found");
                    }
                    else
                    {
                        // For other callbacks like 'Market Locale', we don't need to do anything special,
                        // just send them back in the next request.
                        if (cb.input is { Count: > 0 } && cb.input[0].value is null)
                        {
                            cb.input[0].value = ""; // Ensure value is not null
                        }
                    }
                }
            }
            catch (JsonException jEx)
            {
                _logger?.Invoke($"JSON Deserialization error: {jEx.Message}");
                throw new InvalidOperationException("Could not deserialize authentication response (JSON error)", jEx);
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"Authentication loop error: {ex.Message}");
                throw new InvalidOperationException("Could not process authentication response", ex);
            }
        }

        if (tokenInfo == null)
        {
            _logger?.Invoke("Failed to authenticate after 10 attempts");
            throw new InvalidOperationException("Could not authenticate with Toyota");
        }

        // Step 2: Authorization - CRITICAL: Must use tokenId as cookie
        var authorizeRequest = new HttpRequestMessage(HttpMethod.Get, _settings.AuthorizeUrl);
        authorizeRequest.Headers.Add("cookie", $"iPlanetDirectoryPro={tokenInfo.tokenId}");

        _logger?.Invoke("Sending authorization request with cookie...");
        var authorizeResponse = await _httpClient.SendAsync(authorizeRequest, cancellationToken);

        // Check for both 302 Found (normal redirect) and 200 OK
        if (authorizeResponse.StatusCode != HttpStatusCode.Found &&
            authorizeResponse.StatusCode != HttpStatusCode.OK)
        {
            _logger?.Invoke($"Authorization failed with status code: {authorizeResponse.StatusCode}");
            throw new InvalidOperationException($"Authorization failed. Response: {authorizeResponse.StatusCode}");
        }

        _logger?.Invoke($"Authorization response: {authorizeResponse.StatusCode}");

        // Extract the authentication code from the Location header
        string? authenticationCode = null;

        if (authorizeResponse.Headers.Location != null)
        {
            var locationString = authorizeResponse.Headers.Location.ToString();
            _logger?.Invoke($"Redirect URL: {locationString}");

            // Method 1: Direct string search (very reliable)
            if (locationString.Contains("?code="))
            {
                int codeIndex = locationString.IndexOf("?code=");
                if (codeIndex >= 0)
                {
                    string afterCode = locationString.Substring(codeIndex + 6); // "?code=".Length = 6
                    int endIndex = afterCode.IndexOf('&');
                    authenticationCode = endIndex >= 0 ? afterCode.Substring(0, endIndex) : afterCode;
                    _logger?.Invoke($"Extracted code using direct string search: {authenticationCode.Substring(0, Math.Min(authenticationCode.Length, 10))}...");
                }
            }

            // Method 2: Full manual query string parsing
            if (string.IsNullOrEmpty(authenticationCode))
            {
                int queryIndex = locationString.IndexOf('?');
                if (queryIndex >= 0)
                {
                    var queryString = locationString.Substring(queryIndex + 1);
                    var pairs = queryString.Split('&');
                    foreach (var pair in pairs)
                    {
                        var parts = pair.Split('=');
                        if (parts.Length == 2 && parts[0] == "code")
                        {
                            authenticationCode = parts[1];
                            _logger?.Invoke($"Extracted code using manual query parsing: {authenticationCode.Substring(0, Math.Min(authenticationCode.Length, 10))}...");
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            _logger?.Invoke("Warning: No Location header found in authorization response");
            foreach (var header in authorizeResponse.Headers)
            {
                _logger?.Invoke($"Header: {header.Key} = {string.Join(", ", header.Value)}");
            }
        }

        if (string.IsNullOrEmpty(authenticationCode))
        {
            _logger?.Invoke("Failed to extract authentication code");
            throw new InvalidOperationException("Could not retrieve the authentication code from Authorize URL");
        }

        // Step 3: Get Access Token using the authentication code
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _settings.AccessTokenUrl);
        tokenRequest.Headers.Add("authorization", "basic b25lYXBwOm9uZWFwcA=="); // "oneapp:oneapp" in base64

        var tokenRequestData = new Dictionary<string, string>
        {
            ["client_id"] = "oneapp",
            ["code"] = authenticationCode,
            ["redirect_uri"] = "com.toyota.oneapp:/oauth2Callback",
            ["grant_type"] = "authorization_code",
            ["code_verifier"] = "plain"
        };

        // Use FormUrlEncodedContent for proper encoding
        tokenRequest.Content = new FormUrlEncodedContent(tokenRequestData);

        _logger?.Invoke("Requesting access token...");
        var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);

        if (tokenResponse.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            _logger?.Invoke($"Token request failed: {tokenResponse.StatusCode}, Content: {errorContent}");
            throw new InvalidOperationException($"Could not retrieve access token: {tokenResponse.StatusCode}");
        }

        var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(tokenResponseContent))
        {
            _logger?.Invoke("Token response was empty");
            throw new InvalidOperationException("Empty response from access token URL");
        }

        AccessTokenEndpointResponse? accessTokenData;
        try
        {
            accessTokenData = JsonSerializer.Deserialize<AccessTokenEndpointResponse>(tokenResponseContent, _jsonOptions);

            if (accessTokenData == null)
            {
                _logger?.Invoke("Failed to deserialize token response");
                throw new InvalidOperationException("Could not deserialize access token response");
            }

            if (string.IsNullOrEmpty(accessTokenData.access_token) ||
                string.IsNullOrEmpty(accessTokenData.id_token) ||
                string.IsNullOrEmpty(accessTokenData.refresh_token) ||
                accessTokenData.expires_in == 0)
            {
                _logger?.Invoke("Token response missing required data");
                throw new InvalidOperationException("Incomplete data in access token response");
            }

            _logger?.Invoke($"Successfully received tokens: access_token={accessTokenData.access_token.Substring(0, Math.Min(accessTokenData.access_token.Length, 10))}..., expires_in={accessTokenData.expires_in}");
        }
        catch (JsonException jEx)
        {
            _logger?.Invoke($"JSON Deserialization error in token response: {jEx.Message}");
            throw new InvalidOperationException("Could not deserialize access token response (JSON error)", jEx);
        }
        catch (Exception ex)
        {
            _logger?.Invoke($"Error processing token response: {ex.Message}");
            throw new InvalidOperationException("Could not process access token response", ex);
        }

        await UpdateTokensAsync(accessTokenData, cancellationToken);
        _logger?.Invoke("Authentication completed successfully");
    }

    private async Task RefreshTokensAsync(CancellationToken cancellationToken)
    {
        _logger?.Invoke("Refreshing tokens");

        // Now using the shared _httpClient
        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.AccessTokenUrl);
        // Use lowercase "basic" to match the RestSharp implementation
        request.Headers.Add("authorization", "basic b25lYXBwOm9uZWFwcA==");

        var tokenRequestData = new Dictionary<string, string>
        {
            ["client_id"] = "oneapp",
            ["redirect_uri"] = "com.toyota.oneapp:/oauth2Callback",
            ["grant_type"] = "refresh_token",
            ["code_verifier"] = "plain",
            ["refresh_token"] = _tokenCache?.refresh_token ?? ""
        };

        // Match RestSharp's exact approach by first converting to string
        var formString = await new FormUrlEncodedContent(tokenRequestData).ReadAsStringAsync(cancellationToken);
        request.Content = new StringContent(formString, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        // Match RestSharp's specific status code check
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger?.Invoke($"Token refresh failed: {response.StatusCode}, Content: {errorContent}");
            throw new InvalidOperationException($"Could not refresh token: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenData = JsonSerializer.Deserialize<AccessTokenEndpointResponse>(responseContent, _jsonOptions);

        if (tokenData is null)
            throw new InvalidOperationException("Failed to deserialize token refresh response");

        await UpdateTokensAsync(tokenData, cancellationToken);
        _logger?.Invoke("Token refresh successful");
    }

    private async Task UpdateTokensAsync(AccessTokenEndpointResponse data, CancellationToken cancellationToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(data.id_token);
        var uuid = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "uuid")?.Value;

        if (string.IsNullOrEmpty(uuid))
            throw new InvalidOperationException("Could not extract UUID from token");

        var tokenExpirationTime = DateTime.UtcNow.AddSeconds(data.expires_in);

        _tokenCache = new TokenCacheItem
        {
            access_token = data.access_token,
            refresh_token = data.refresh_token,
            uuid = uuid,
            expiration = tokenExpirationTime,
            username = _username
        };

        if (_useTokenCaching)
        {
            var json = JsonSerializer.Serialize(_tokenCache, _jsonOptions);
            await File.WriteAllTextAsync(_tokenCacheFilename, json, cancellationToken);
        }
    }

    private bool IsTokenValid() =>
        _tokenCache is { access_token: not null } && _tokenCache.expiration > DateTime.UtcNow;

    // Request building methods
    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, string? vin = null)
    {
        // Ensure we're using the full URL like RestSharp does
        var fullUrl = endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? endpoint
            : $"{_settings.ApiBaseUrl}{endpoint}";
        var request = new HttpRequestMessage(method, fullUrl);

        var uuid4 = Guid.NewGuid().ToString("D").ToUpperInvariant();

        // Add the API key headers only when a key is configured. HttpRequestHeaders.Add
        // throws ArgumentException for empty values, so omitting the headers entirely is safer
        // than silently sending an empty credential to the API.
        if (string.IsNullOrEmpty(_apiKey))
        {
            if (!_warnedAboutMissingApiKey)
            {
                _warnedAboutMissingApiKey = true;
                _logger?.Invoke("No Toyota API key configured. Set 'ToyotaApi:ApiKey' or the 'TOYOTA_API_KEY' environment variable.");
            }
        }
        else
        {
            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("API_KEY", _apiKey);
        }

        if (_tokenCache is not null)
        {
            request.Headers.Add("x-guid", _tokenCache.uuid);
            request.Headers.Add("guid", _tokenCache.uuid);
            request.Headers.Add("x-client-ref", GenerateHmacSha256(_settings.ClientVersion, _tokenCache.uuid ?? ""));
        }

        request.Headers.Add("x-correlationid", uuid4);
        request.Headers.Add("x-appversion", _settings.ClientVersion);
        request.Headers.Add("x-region", "EU");

        if (_tokenCache?.access_token is not null)
        {
            request.Headers.Add("authorization", $"Bearer {_tokenCache.access_token}");
        }

        request.Headers.Add("x-channel", "ONEAPP");
        request.Headers.Add("x-brand", "T");
        request.Headers.Add("user-agent", "okhttp/4.10.0"); // Ensure this exactly matches RestSharp's user-agent

        if (vin is not null)
            request.Headers.Add("vin", vin);

        return request;
    }

    private static string GenerateHmacSha256(string key, string message)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private Task<T?> SendRequestAsync<T>(HttpMethod method, string endpoint, string? vin = null, CancellationToken cancellationToken = default) =>
        SendCoreAsync<T>(method, endpoint, vin, content: null, cancellationToken);

    private Task<T?> SendRequestWithBodyAsync<T>(HttpMethod method, string endpoint, object body, string? vin = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        return SendCoreAsync<T>(method, endpoint, vin, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);
    }

    private async Task<T?> SendCoreAsync<T>(HttpMethod method, string endpoint, string? vin, HttpContent? content, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(method, endpoint, vin);

        if (content is null && method == HttpMethod.Post)
        {
            // The Toyota API expects a JSON body (even an empty one) on POST requests.
            content = new StringContent("", Encoding.UTF8, "application/json");
        }
        request.Content = content;

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Found)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger?.Invoke($"API {method} request failed: {response.StatusCode}, Content: {errorContent}");
            throw new InvalidOperationException($"Could not retrieve information from Toyota API: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }

    // Public API methods

    /// <inheritdoc />
    public Task<VehiclesModel?> GetVehiclesAsync(CancellationToken cancellationToken = default) =>
        SendRequestAsync<VehiclesModel>(HttpMethod.Get, _settings.VehicleGuidEndpoint, null, cancellationToken);

    /// <inheritdoc />
    public Task<ElectricResponseModel?> GetElectricAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<ElectricResponseModel>(HttpMethod.Get, _settings.VehicleGlobalRemoteElectricStatusEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<RealtimeStatus?> GetElectricRealtimeStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<RealtimeStatus>(HttpMethod.Post, _settings.VehicleGlobalRemoteElectricRealtimeStatusEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<LocationResponseModel?> GetLocationAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<LocationResponseModel>(HttpMethod.Get, _settings.VehicleLocationEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<HealthStatusResponseModel?> GetHealthStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<HealthStatusResponseModel>(HttpMethod.Get, _settings.VehicleHealthStatusEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<TelemetryStatusResponseModel?> GetTelemetryStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<TelemetryStatusResponseModel>(HttpMethod.Get, _settings.VehicleTelemetryEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<NotificationsResponseModel?> GetNotificationsAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<NotificationsResponseModel>(HttpMethod.Get, _settings.VehicleNotificationHistoryEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<RemoteStatusResponseModel?> GetRemoteStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<RemoteStatusResponseModel>(HttpMethod.Get, _settings.VehicleGlobalRemoteStatusEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<ServiceHistoryResponseModel?> GetServiceHistoryAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<ServiceHistoryResponseModel>(HttpMethod.Get, _settings.VehicleServiceHistoryEndpoint, vin, cancellationToken);

    // --- Trips ---

    /// <inheritdoc />
    public Task<TripsResponseModel?> GetTripsAsync(string vin, DateOnly from, DateOnly to, bool route = false, bool summary = true, int limit = 50, int offset = 0, CancellationToken cancellationToken = default)
    {
        var endpoint = _settings.VehicleTripsEndpoint
            .Replace("{from_date}", from.ToString("yyyy-MM-dd"))
            .Replace("{to_date}", to.ToString("yyyy-MM-dd"))
            .Replace("{route}", route.ToString().ToLowerInvariant())
            .Replace("{summary}", summary.ToString().ToLowerInvariant())
            .Replace("{limit}", limit.ToString())
            .Replace("{offset}", offset.ToString());

        return SendRequestAsync<TripsResponseModel>(HttpMethod.Get, endpoint, vin, cancellationToken);
    }

    // --- Climate Control ---

    /// <inheritdoc />
    public Task<ClimateSettingsResponseModel?> GetClimateSettingsAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<ClimateSettingsResponseModel>(HttpMethod.Get, _settings.VehicleClimateSettingsEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<ClimateStatusResponseModel?> GetClimateStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<ClimateStatusResponseModel>(HttpMethod.Get, _settings.VehicleClimateStatusEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<ClimateControlResponseModel?> RefreshClimateStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<ClimateControlResponseModel>(HttpMethod.Post, _settings.VehicleClimateStatusRefreshEndpoint, vin, cancellationToken);

    /// <inheritdoc />
    public Task<ClimateControlResponseModel?> StartClimateControlAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestWithBodyAsync<ClimateControlResponseModel>(HttpMethod.Post, _settings.VehicleClimateControlEndpoint, new { command = "start" }, vin, cancellationToken);

    /// <inheritdoc />
    public Task<ClimateControlResponseModel?> StopClimateControlAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestWithBodyAsync<ClimateControlResponseModel>(HttpMethod.Post, _settings.VehicleClimateControlEndpoint, new { command = "stop" }, vin, cancellationToken);

    // --- Remote Commands ---

    /// <inheritdoc />
    public Task<RemoteCommandResponseModel?> SendRemoteCommandAsync(string vin, RemoteCommandType command, CancellationToken cancellationToken = default) =>
        SendRequestWithBodyAsync<RemoteCommandResponseModel>(HttpMethod.Post, _settings.VehicleCommandEndpoint, new { command = command.ToString() }, vin, cancellationToken);

    // --- Vehicle Association ---

    /// <inheritdoc />
    public Task<VehicleAssociationResponseModel?> GetVehicleAssociationAsync(CancellationToken cancellationToken = default) =>
        SendRequestAsync<VehicleAssociationResponseModel>(HttpMethod.Get, _settings.VehicleAssociationEndpoint, null, cancellationToken);

    // --- Driving Statistics ---

    /// <inheritdoc />
    public Task<DrivingStatisticsResponseModel?> GetDrivingStatisticsAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<DrivingStatisticsResponseModel>(HttpMethod.Get, _settings.VehicleDrivingStatisticsEndpoint, vin, cancellationToken);

    // --- Lock Status ---

    /// <inheritdoc />
    public Task<LockStatusResponseModel?> GetLockStatusAsync(string vin, CancellationToken cancellationToken = default) =>
        SendRequestAsync<LockStatusResponseModel>(HttpMethod.Get, _settings.VehicleLockStatusEndpoint, vin, cancellationToken);
}

