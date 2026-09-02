using DJI.Bl.Mappers;
using DJI.Bl.Models;
using DJI.Contracts.Rss;
using DJI.Core.Entities;
using DJI.Core.Enums;
using DJI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DJI.Bl.Services;

public interface IFiltersService
{
    Task<FiltersRs> GetAsync(CancellationToken ct = default);
}

public class FiltersService(
    IRepository<Manager> managers,
    IRepository<Category> categories,
    IRepository<Sale> sales) : IFiltersService
{
    public async Task<FiltersRs> GetAsync(CancellationToken ct = default)
    {
        var managerNames = await managers.GetAllAsync(
            manager => new ManagerName(manager.Id, manager.FirstName, manager.LastName),
            sorter: query => query.OrderBy(manager => manager.LastName),
            ct: ct);

        var categoryOptions = await categories.GetAllAsync(
            category => new FilterOptionRs(category.Id, category.Name),
            sorter: query => query.OrderBy(category => category.Name),
            ct: ct);

        var bounds = await sales.Query()
            .GroupBy(_ => 1)
            .Select(group => new
            {
                First = group.Min(sale => (DateOnly?)sale.SaleDate),
                Last = group.Max(sale => (DateOnly?)sale.SaleDate),
            })
            .FirstOrDefaultAsync(ct);

        var managerOptions = managerNames
            .Select(manager => new FilterOptionRs(
                manager.Id,
                ManagerMapper.FullName(manager.FirstName, manager.LastName)))
            .ToList();

        return new FiltersRs(
            managerOptions,
            categoryOptions,
            Enum.GetNames<SaleStatusEnum>(),
            bounds?.First,
            bounds?.Last);
    }
}
