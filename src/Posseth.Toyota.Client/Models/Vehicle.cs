using System.Collections.Generic;

namespace Posseth.Toyota.Client.Models
{
    public class Vehicle
    {
        public object? RegistrationNumber { get; set; }
        public string? Vin { get; set; }
        public string? ModelYear { get; set; }
        public string? ModelName { get; set; }
        public string? ModelDescription { get; set; }
        public string? ModelCode { get; set; }
        public string? Region { get; set; }
        public string? Status { get; set; }
        public string? Generation { get; set; }
        public string? Image { get; set; }
        public string? NickName { get; set; }
        public string? Brand { get; set; }
        public object? HwType { get; set; }
        public string? AsiCode { get; set; }
        public string? SubscriptionStatus { get; set; }
        public bool PrimarySubscriber { get; set; }
        public bool RemoteUser { get; set; }
        public bool EvVehicle { get; set; }
        public string? RemoteSubscriptionStatus { get; set; }
        public string? RemoteUserGuid { get; set; }
        public string? SubscriberGuid { get; set; }
        public string? RemoteDisplay { get; set; }
        public object? EmergencyContact { get; set; }
        public RemoteServiceCapabilities? RemoteServiceCapabilities { get; set; }
        public bool NonCvtVehicle { get; set; }
        public List<object>? RemoteServicesExceptions { get; set; }
        public ExtendedCapabilities? ExtendedCapabilities { get; set; }
        public string? ElectricalPlatformCode { get; set; }
        public PersonalizedSettings? PersonalizedSettings { get; set; }
        public string? FuelType { get; set; }
        public object? FleetInd { get; set; }
        public bool SvlStatus { get; set; }
        public string? DisplayModelDescription { get; set; }
        public string? ManufacturerCode { get; set; }
        public object? SuffixCode { get; set; }
        public string? Imei { get; set; }
        public string? Color { get; set; }
        public object? VehicleDataConsents { get; set; }
        public DataConsent? DataConsent { get; set; }
        public List<object>? VehicleCapabilities { get; set; }
        public List<Capability>? Capabilities { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<object>? Services { get; set; }
        public List<DisplaySubscription>? DisplaySubscriptions { get; set; }
        public List<object>? Alerts { get; set; }
        public Features? Features { get; set; }
        public bool SubscriptionExpirationStatus { get; set; }
        public string? FaqUrl { get; set; }
        public object? ShopGenuinePartsUrl { get; set; }
        public bool FamilySharing { get; set; }
        public CtsLinks? CtsLinks { get; set; }
        public TffLinks? TffLinks { get; set; }
        public HeadUnit? HeadUnit { get; set; }
        public string? KatashikiCode { get; set; }
        public object? OldImei { get; set; }
        public string? StockPicReference { get; set; }
        public string? TransmissionType { get; set; }
        public string? DateOfFirstUse { get; set; }
        public Dcm? Dcm { get; set; }
        public List<object>? Dcms { get; set; }
        public string? ManufacturedDate { get; set; }
        public string? ContractId { get; set; }
        public string? CarlineName { get; set; }
        public object? ExternalSubscriptions { get; set; }
        public object? ServiceConnectStatus { get; set; }
        public object? ProXSeatsVideoLink { get; set; }
        public bool Owner { get; set; }
        public bool CommercialRental { get; set; }
        public bool RemoteSubscriptionExists { get; set; }
        public bool DcmActive { get; set; }
        public int Preferred { get; set; }
    }
}