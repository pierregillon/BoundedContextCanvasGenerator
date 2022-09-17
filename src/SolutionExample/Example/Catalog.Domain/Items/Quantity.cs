namespace Catalog.Domain.Items;

public record Quantity(int Value)
{
    public static Quantity operator -(Quantity first, Quantity second) => new(first.Value - second.Value);
}