using System.Collections.Generic;

namespace Posseth.Toyota.Client.Models
{
    public record PersonalizedSettings(
        object? Name,
        object? Link,
        object? ImageUrl,
        object? Body,
        object? ButtonText
    );
}