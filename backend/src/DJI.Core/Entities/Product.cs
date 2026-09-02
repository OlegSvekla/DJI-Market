namespace DJI.Core.Entities;

public class Product : Entity
{
    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public decimal ListPrice { get; set; }

    public decimal BaseCost { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = [];
}
