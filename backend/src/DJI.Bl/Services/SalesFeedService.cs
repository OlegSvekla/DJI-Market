using DJI.Bl.Mappers;
using DJI.Bl.Models;
using DJI.Contracts.Rss;
using DJI.Core.Analytics;
using DJI.Core.Entities;
using DJI.Core.Enums;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface ISalesFeedService
{
    Task<PagedRs<RecentSaleRs>> GetRecentAsync(
        Period period,
        int page,
        int pageSize,
        int? managerId,
        SaleStatusEnum? status,
        CancellationToken ct = default);
}

public class SalesFeedService(IRepository<Sale> sales) : ISalesFeedService
{
    private const int MaxPageSize = 100;

    public async Task<PagedRs<RecentSaleRs>> GetRecentAsync(
        Period period,
        int page,
        int pageSize,
        int? managerId,
        SaleStatusEnum? status,
        CancellationToken ct = default)
    {
        var safePage = Math.Max(1, page);
        var safePageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = sales.Query().InPeriod(period);

        if (managerId is not null)
        {
            query = query.Where(sale => sale.ManagerId == managerId);
        }

        if (status is not null)
        {
            query = query.Where(sale => sale.Status == status);
        }

        var total = await query.CountAsync(ct);

        var rows = await query
            .OrderByDescending(sale => sale.SaleDate)
            .ThenByDescending(sale => sale.Id)
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .Select(sale => new SaleRow(
                sale.Id,
                sale.Number,
                sale.SaleDate,
                new ManagerProfile(
                    sale.ManagerId,
                    sale.Manager.FirstName,
                    sale.Manager.LastName,
                    sale.Manager.Team,
                    sale.Manager.AvatarColor,
                    sale.Manager.IsActive),
                sale.Customer.Company,
                sale.Customer.Name,
                sale.Status,
                sale.Items.Count,
                sale.Items
                    .OrderByDescending(item => item.Quantity * item.UnitPrice)
                    .Select(item => item.Product.Name)
                    .FirstOrDefault(),
                sale.Items.Sum(item => item.Quantity * item.UnitPrice),
                sale.Items.Sum(item => item.Quantity * (item.UnitPrice - item.UnitCost))))
            .ToListAsync(ct);

        return new PagedRs<RecentSaleRs>(
            [.. rows.Select(SaleMapper.ToRecentSale)],
            safePage,
            safePageSize,
            total);
    }
}
