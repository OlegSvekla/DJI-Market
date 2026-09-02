using DJI.Core.Enums;

namespace DJI.Bl.Models;

public sealed record SaleRow(
    int Id,
    string Number,
    DateOnly Date,
    ManagerProfile Manager,
    string CustomerCompany,
    string CustomerName,
    SaleStatusEnum Status,
    int ItemsCount,
    string? TopItemName,
    decimal Amount,
    decimal GrossProfit);
