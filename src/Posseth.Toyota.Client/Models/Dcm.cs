namespace Posseth.Toyota.Client.Models
{
    public record Dcm(
        string? DcmModelYear,
        string? DcmDestination,
        string? CountryCode,
        string? DcmSupplier,
        string? DcmSupplierName,
        string? DcmGrade,
        string? Euiccid,
        object? HardwareType,
        object? VehicleUnitTerminalNumber
    );
}