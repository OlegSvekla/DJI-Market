namespace DJI.Bl.Models;

public sealed record ManagerProfile(
    int Id,
    string FirstName,
    string LastName,
    string Team,
    string AvatarColor,
    bool IsActive);
