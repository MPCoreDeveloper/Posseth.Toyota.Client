namespace Posseth.Toyota.Client.Models
{
    public record HeadUnit(
        string? MobilePlatformCode,
        string? HuDescription,
        string? HuGeneration,
        string? HuVersion,
        string? MultimediaType,
        string? DeviceId
    );
}