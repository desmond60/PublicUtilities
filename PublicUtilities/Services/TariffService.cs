using Microsoft.EntityFrameworkCore;
using PublicUtilities.Data;
using PublicUtilities.Dtos.Tariffs;
using PublicUtilities.Models;

namespace PublicUtilities.Services;

public interface ITariffService
{
    Task<CreateTariffResultDto> CreateTariff(CreateTariffDto user);
    Task<DeleteTariffResultDto> DeleteTariff(DeleteTariffDto user);
    Task<List<Tariff>> GetAllTariffs();
}

public class TariffService : ITariffService
{
    private readonly AppDbContext _context;

    public TariffService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTariffResultDto> CreateTariff(CreateTariffDto tariff)
    {
        var newTariff = new Tariff()
        {
            ServiceType = tariff.ServiceType,
            Component = tariff.Component,
            Price = tariff.Price,
            Normative = tariff.Normative,
            Unit = tariff.Unit,
        };

        _context.Tariffs.Add(newTariff);
        await _context.SaveChangesAsync();

        return new CreateTariffResultDto() { IsAdd = true, Message = "Тариф успешно добавлен" };
    }

    public async Task<DeleteTariffResultDto> DeleteTariff(DeleteTariffDto tariff)
    {
        var delTariff = tariff.Component is null ? _context.Tariffs.FirstOrDefault(x => x.ServiceType == tariff.ServiceType)
            : _context.Tariffs.FirstOrDefault(x => x.ServiceType == tariff.ServiceType && x.Component == tariff.Component);
        if (delTariff is null) return new DeleteTariffResultDto() { IsDeleted = false, Message = $"Тариф с такой услугой {tariff.ServiceType} не найден" };

        _context.Tariffs.Remove(delTariff);
        await _context.SaveChangesAsync();

        return new DeleteTariffResultDto() { IsDeleted = true, Message = $"Тариф с услугой {tariff.ServiceType} удален" };
    }

    public async Task<List<Tariff>> GetAllTariffs()
    {
        var result = await _context.Tariffs.ToListAsync();
        return result;
    }
}
