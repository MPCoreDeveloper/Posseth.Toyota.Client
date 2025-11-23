namespace Posseth.Toyota.Client.Models
{
    public record ToyotaApiResponse(
        string? Status,
        string? Message
        // Voeg hier extra properties toe afhankelijk van de API response
    );
}