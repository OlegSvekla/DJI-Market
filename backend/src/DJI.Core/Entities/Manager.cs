namespace DJI.Core.Entities;

public class Manager : Entity
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Team { get; set; } = null!;

    public string Position { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateOnly HiredOn { get; set; }

    public string AvatarColor { get; set; } = null!;

    public ICollection<Sale> Sales { get; set; } = [];
}
