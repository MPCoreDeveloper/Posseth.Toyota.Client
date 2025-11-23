namespace Posseth.Toyota.Client.Models
{
    public record RemoteServiceCapabilities(
        bool GuestDriverCapable,
        bool VehicleFinderCapable,
        bool EstartStopCapable,
        bool EstartEnabled,
        bool EstopEnabled,
        bool HazardCapable,
        bool DlockUnlockCapable,
        bool HeadLightCapable,
        bool AcsettingEnabled,
        bool TrunkCapable,
        bool PowerWindowCapable,
        bool VentilatorCapable,
        bool SteeringWheelHeaterCapable,
        bool AllowHvacOverrideCapable,
        bool MoonRoofCapable
    );
}