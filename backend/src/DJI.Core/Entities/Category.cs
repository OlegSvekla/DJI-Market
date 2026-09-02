namespace DJI.Core.Entities;

public class Category : Entity
{
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = [];
}
