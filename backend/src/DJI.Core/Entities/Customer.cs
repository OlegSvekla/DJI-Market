using DJI.Core.Enums;

namespace DJI.Core.Entities;

public class Customer : Entity
{
    public string Name { get; set; } = null!;

    public string Company { get; set; } = null!;

    public CustomerSegmentEnum Segment { get; set; }

    public ICollection<Sale> Sales { get; set; } = [];
}
