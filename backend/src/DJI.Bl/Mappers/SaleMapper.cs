using DJI.Bl.Models;
using DJI.Contracts.Rss;

namespace DJI.Bl.Mappers;

public static class SaleMapper
{
    public static RecentSaleRs ToRecentSale(SaleRow row) => new(
        row.Id,
        row.Number,
        row.Date,
        ManagerMapper.ToBrief(row.Manager),
        row.CustomerCompany,
        row.CustomerName,
        row.Status,
        row.ItemsCount,
        row.TopItemName ?? string.Empty,
        row.Amount,
        row.GrossProfit);
}
