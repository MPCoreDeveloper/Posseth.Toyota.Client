namespace Posseth.Toyota.Client.Models
{
    public record Message(
        string? ResponseCode,
        string? Description,
        string? DetailedDescription
    );
}