using System.Collections.Generic;

namespace Posseth.Toyota.Client.Models
{
    public record Subscription(
        string? SubscriptionID,
        string? ProductName,
        string? ProductDescription,
        string? DisplayProductName,
        string? SubscriptionEndDate,
        object? SubscriptionNextBillingDate,
        string? SubscriptionStartDate,
        string? Status,
        string? Type,
        int SubscriptionRemainingDays,
        object? SubscriptionRemainingTerm,
        string? ProductCode,
        string? ProductLine,
        object? ProductType,
        int Term,
        string? TermUnit,
        object? GoodwillIssuedFor,
        bool Renewable,
        string? DisplayTerm,
        string? SubscriptionTerm,
        bool AutoRenew,
        bool FutureCancel,
        List<object>? ConsolidatedProductIds,
        List<object>? ConsolidatedGoodwillIds,
        object? Components,
        string? Category
    );
}