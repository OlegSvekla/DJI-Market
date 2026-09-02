using DJI.Core.Enums;

namespace DJI.Core.Entities;

public class Sale : Entity
{
    public string Number { get; set; } = null!;

    public int ManagerId { get; set; }

    public Manager Manager { get; set; } = null!;

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public DateOnly SaleDate { get; set; }

    public SaleStatusEnum Status { get; set; }

    public ICollection<SaleItem> Items { get; set; } = [];
}
