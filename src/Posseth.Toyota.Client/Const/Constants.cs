namespace Posseth.Toyota.Client.Const;

/// <summary>
/// Fixed constants used by the client.
/// </summary>
/// <remarks>
/// API URLs and endpoints are no longer part of this class. They live in
/// <see cref="ToyotaApiSettings"/> so they can be overridden via configuration or DI.
/// </remarks>
public static class Constants
{
    // Client version
    public const string CLIENT_VERSION = "2.14.0";

    // Units
    public const string KILOMETERS_UNIT = "km";
    public const string MILES_UNIT = "mi";
    public const double L_TO_MPG_FACTOR = 235.215;
    public const double ML_TO_L_FACTOR = 1000.0;
    public const double ML_TO_GAL_FACTOR = 3785.0;
    public const double KM_TO_MILES_FACTOR = 0.621371192;
    public const double MILES_TO_KM_FACTOR = 1.60934;
}
