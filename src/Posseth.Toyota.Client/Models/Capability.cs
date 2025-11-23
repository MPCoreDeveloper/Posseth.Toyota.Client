using System.Text.Json.Serialization;

namespace Posseth.Toyota.Client.Models
{
    public record Capability(
        string? Name,
        object? Description,
        bool Value,
        Translation? Translation,
        bool Display,
        object? DisplayName
    );
}