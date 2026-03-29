using System.Text.Json.Serialization;

namespace Posseth.Toyota.Client.Models
{
    /// <summary>
    /// Remote commands that can be sent to the vehicle via the Toyota Connected Services API.
    /// Check <see cref="RemoteServiceCapabilities"/> on the <see cref="Vehicle"/> to verify
    /// whether the vehicle supports a given command before sending it.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RemoteCommandType
    {
        /// <summary>Lock all doors. Requires <see cref="RemoteServiceCapabilities.DlockUnlockCapable"/>.</summary>
        DoorLock,

        /// <summary>Unlock all doors. Requires <see cref="RemoteServiceCapabilities.DlockUnlockCapable"/>.</summary>
        DoorUnlock,

        /// <summary>Start the engine remotely. Requires <see cref="RemoteServiceCapabilities.EstartStopCapable"/> and <see cref="RemoteServiceCapabilities.EstartEnabled"/>.</summary>
        EngineStart,

        /// <summary>Stop the engine remotely. Requires <see cref="RemoteServiceCapabilities.EstartStopCapable"/> and <see cref="RemoteServiceCapabilities.EstopEnabled"/>.</summary>
        EngineStop,

        /// <summary>Turn hazard lights on. Requires <see cref="RemoteServiceCapabilities.HazardCapable"/>.</summary>
        HazardOn,

        /// <summary>Turn hazard lights off. Requires <see cref="RemoteServiceCapabilities.HazardCapable"/>.</summary>
        HazardOff,

        /// <summary>Turn headlights on. Requires <see cref="RemoteServiceCapabilities.HeadLightCapable"/>.</summary>
        HeadLightOn,

        /// <summary>Turn headlights off. Requires <see cref="RemoteServiceCapabilities.HeadLightCapable"/>.</summary>
        HeadLightOff,

        /// <summary>Open the trunk. Requires <see cref="RemoteServiceCapabilities.TrunkCapable"/>.</summary>
        TrunkOpen
    }
}
