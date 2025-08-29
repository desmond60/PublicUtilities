using Microsoft.EntityFrameworkCore;
using PublicUtilities.Data;
using PublicUtilities.Dtos.UtilitiesCalculation;
using PublicUtilities.Models;

namespace PublicUtilities.Services;

public interface IUtilitiesCalcService
{
    Task<UtilitiesCalcResultDto> UtilitiesCalcProcess(UtilitiesCalcDto utilities, List<ResidentPeriodDto> periods);
}

public class UtilitiesCalcService : IUtilitiesCalcService
{
    private readonly AppDbContext _context;

    public UtilitiesCalcService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UtilitiesCalcResultDto> UtilitiesCalcProcess(UtilitiesCalcDto utilities, List<ResidentPeriodDto> periods)
    {
        var validResult = IsValidate(utilities, periods);
        if (!validResult.isValid)
            return new UtilitiesCalcResultDto() { IsCompleted = false, Message = "Ошибка валидации", Errors = validResult.errors };

        var user = await _context.Users
                .Include(u => u.ResidentPeriods)
                .Include(u => u.Metrics)
                .Include(u => u.СalculationResults)
                .FirstOrDefaultAsync(u => u.PersonalAccount == utilities.PersonalAccount);
        if (user is null) return new UtilitiesCalcResultDto() { IsCompleted = false, Message = $"Пользователь с лицевым счетом {utilities.PersonalAccount} не найден" };

        var tariffs = await _context.Tariffs.ToListAsync();
        if (tariffs is null) return new UtilitiesCalcResultDto() { IsCompleted = false, Message = $"Не смогли получить тарифы" };

        // Метрики за рпедыдущий месяц
        var previousUtilities = user.Metrics.FirstOrDefault(m => m.Date == utilities.Date.AddMonths(-1)) ?? new Metric();
        
        List<MetricCalculated> metrics = new List<MetricCalculated>();

        // Расчет услуги ХВС
        metrics.Add(ColdWaterCalculation(utilities.ColdWater, previousUtilities.ColdWater, tariffs, periods));

        // Расчет услуги ГВС
        metrics.AddRange(HotWaterCalculation(utilities.HotWater, previousUtilities.HotWater, tariffs, periods));

        // Расчет услуги ЭЭ
        metrics.AddRange(ElectricityCalculation(utilities.ElectricityDay, previousUtilities.ElectricityDay, utilities.ElectricityNight, previousUtilities.ElectricityNight, tariffs, periods));

        // Сохранение в БД, если необходимо
        UtilitiesCalcResultDto result;
        if (utilities.SaveToDb)
        {
            await SaveDataAsync(user, utilities, metrics, periods);

            result = new UtilitiesCalcResultDto()
            {
                IsCompleted = true,
                Message = "Расчеты произведены и сохранены в БД",
                Metrics = metrics,
                TotalSum = metrics.Select(m => m.Price).Sum()
            };
            return result;
        }

        result = new UtilitiesCalcResultDto()
        {
            IsCompleted = true,
            Message = "Расчеты произведены",
            Metrics = metrics,
            TotalSum = metrics.Select(m => m.Price).Sum()
        };
        return result;
    }

    public async Task SaveDataAsync(User user, UtilitiesCalcDto utilities, List<MetricCalculated> metrics, List<ResidentPeriodDto> periods)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        // Если нет хоть одного прибора, сохраняем периоды
        bool noMetrics =
            utilities.ColdWater == 0 ||
            utilities.HotWater == 0 ||
            utilities.ElectricityDay == 0 ||
            utilities.ElectricityNight == 0;
        if (noMetrics)
            for (int i = 0; i < periods.Count; i++)
            {
                user.ResidentPeriods.Add(new ResidentPeriod()
                {
                    StartDate = periods[i].StartDate,
                    EndDate = periods[i].EndDate,
                    ResidentCount = periods[i].ResidentCount,
                });
            }

        user.Metrics.Add(new Metric()
        {
            Date = utilities.Date,
            ColdWater = utilities.ColdWater,
            HotWater = utilities.HotWater,
            ElectricityDay = utilities.ElectricityDay,
            ElectricityNight = utilities.ElectricityNight,
        });

        for (int i = 0; i < metrics.Count; i++)
        {
            user.СalculationResults.Add(new CalculationResult()
            {
                Date = utilities.Date,
                ServiceType = metrics[i].ServiceType,
                Component = metrics[i].Component,
                Volume = metrics[i].Volume,
                Tariff = metrics[i].Tariff,
                Price = metrics[i].Price,
            });
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public List<MetricCalculated> ElectricityCalculation(double currentMonthDay, double? previousMonthDay, double currentMonthNight, double? previousMonthNight, List<Tariff> tariffs, List<ResidentPeriodDto> periods)
    {
        Tariff tariffDay = tariffs.FirstOrDefault(t => t.ServiceType == "ЭЭ" && t.Component == "день")!;
        Tariff tariffNight = tariffs.FirstOrDefault(t => t.ServiceType == "ЭЭ" && t.Component == "ночь")!;
        Tariff tariffElectricity = tariffs.FirstOrDefault(t => t.ServiceType == "ЭЭ" && t.Component is null)!;

        List<MetricCalculated> metrics = new List<MetricCalculated>();
        if (currentMonthDay == 0 && currentMonthNight == 0)
        {
            double volume = GetStandartVolume(tariffElectricity, periods);
            double price = volume * tariffElectricity.Price;

            metrics.Add(new MetricCalculated()
            {
                ServiceType = tariffElectricity.ServiceType,
                Component = tariffElectricity.Component,
                Tariff = tariffElectricity.Price,
                Volume = volume,
                Price = price
            });
            return metrics;
        }
        else
        {
            double volumeMonthDay = currentMonthDay - previousMonthDay ?? 0.0;
            double priceMonthDay = volumeMonthDay * tariffDay.Price;

            double volumeMonthNight = currentMonthNight - previousMonthNight ?? 0.0;
            double priceMonthNight = volumeMonthNight * tariffNight.Price;

            metrics.AddRange([
                new MetricCalculated()
                {
                    ServiceType = tariffDay.ServiceType,
                    Component = tariffDay.Component,
                    Tariff = tariffDay.Price,
                    Volume = volumeMonthDay,
                    Price = priceMonthDay
                },
                new MetricCalculated()
                {
                    ServiceType = tariffNight.ServiceType,
                    Component = tariffNight.Component,
                    Tariff = tariffNight.Price,
                    Volume = volumeMonthNight,
                    Price = priceMonthNight
                }
            ]);

            return metrics;
        }
    }

    public List<MetricCalculated> HotWaterCalculation(double currentMonth, double? previousMonth, List<Tariff> tariffs, List<ResidentPeriodDto> periods)
    {
        Tariff tariffCarrier = tariffs.FirstOrDefault(t => t.ServiceType == "ГВС" && t.Component == "ТН")!;
        Tariff tariffEnergy = tariffs.FirstOrDefault(t => t.ServiceType == "ГВС" && t.Component == "ТЭ")!;

        // Расчет "ГВС Теплоноситель"
        double volumeCarrier = currentMonth != 0 ? currentMonth - previousMonth ?? 0.0 : GetStandartVolume(tariffCarrier, periods);
        double priceCarrier = volumeCarrier * tariffCarrier.Price;

        // Расчет "ГВС Тепловая энергия"
        double volumeEnergy = (double)(volumeCarrier * tariffEnergy.Normative)!;
        double priceEnergy = volumeEnergy * tariffEnergy.Price;

        MetricCalculated metricCarrier = new MetricCalculated()
        {
            ServiceType = tariffCarrier.ServiceType,
            Component = tariffCarrier.Component,
            Tariff = tariffCarrier.Price,
            Volume = volumeCarrier,
            Price = priceCarrier
        };

        MetricCalculated metricEnergy = new MetricCalculated()
        {
            ServiceType = tariffEnergy.ServiceType,
            Component = tariffEnergy.Component,
            Tariff = tariffEnergy.Price,
            Volume = volumeEnergy,
            Price = priceEnergy
        };

        return [metricCarrier, metricEnergy];
    }

    public MetricCalculated ColdWaterCalculation(double currentMonth, double? previousMonth, List<Tariff> tariffs, List<ResidentPeriodDto> periods)
    {
        Tariff tariff = tariffs.FirstOrDefault(t => t.ServiceType == "ХВС")!;

        double volume = currentMonth != 0 ? currentMonth - previousMonth ?? 0.0 : GetStandartVolume(tariff, periods);
        double price = volume * tariff.Price;

        return new MetricCalculated() {
            ServiceType = tariff.ServiceType,
            Component = tariff.Component,
            Tariff = tariff.Price,
            Volume = volume,
            Price = price
        };
    }

    public double GetStandartVolume(Tariff tariff, List<ResidentPeriodDto> periods)
    {
        var firstPeriod = periods.First();
        int days = DateTime.DaysInMonth(firstPeriod.StartDate.Year, firstPeriod.StartDate.Month);
        int humanDays = firstPeriod.ResidentCount * firstPeriod.EndDate.Day;
        for (int i = 1; i < periods.Count; i++)
            humanDays += periods[i].ResidentCount * (periods[i].EndDate.Day - periods[i].StartDate.Day + 1);

        return (double)(tariff.Normative * ((double)humanDays / days))!;
    }

    public (bool isValid, List<string> errors) IsValidate(UtilitiesCalcDto utilities, List<ResidentPeriodDto> periods)
    {
        List<string> errors = new List<string>();

        if (((utilities.ElectricityDay == 0) ^ (utilities.ElectricityNight == 0)))
            errors.Add($"Показания электроэнергии должны быть либо оба 0 (если нет прибора), либо оба > 0 (если имеется прибор)");

        if (periods is not null)
            for (int i = 0; i < periods.Count; i++)
            {
                if (periods[i].ResidentCount <= 0)
                    errors.Add($"[Период {i + 1}] Количество жильцов должно быть больше 0");

                if (periods[i].StartDate > periods[i].EndDate)
                    errors.Add($"[Период {i + 1}] Дата начала больше даты окончания");
            }

        if (errors.Any())
            return (false, errors);

        bool noMetrics =
            utilities.ColdWater == 0 ||
            utilities.HotWater == 0 ||
            utilities.ElectricityDay == 0 ||
            utilities.ElectricityNight == 0;

        // Если указаны все показания, то периоды можно не проверять
        if (!noMetrics)
            return (true, errors);

        if (noMetrics && (periods is null || periods.Count == 0))
        {
            errors.Add("Так как показания отсутствуют, необходимо указать периоды проживания. Если периоды указаны, проверьте даты на правильность");
            return (false, errors);
        }

        var sortedPeriods = periods.OrderBy(p => p.StartDate).ToArray();
        var startMonth = utilities.Date;
        var endMonth = utilities.Date.AddMonths(1).AddDays(-1);

        // Проверка на пересечение
        for (int i = 1; i < sortedPeriods.Length; i++)
        {
            if (sortedPeriods[i].StartDate < sortedPeriods[i - 1].EndDate)
            {
                errors.Add($"Периоды пересекаются. [Период {i + 1}] Дата начала: {sortedPeriods[i].StartDate} < [Период {i}] Дата окончания: {sortedPeriods[i - 1].EndDate}");
                return (false, errors);
            }
        }

        // Проверка, что месяц полностью покрыт
        if (sortedPeriods.First().StartDate != startMonth)
        {
            errors.Add($"Период не покрывает месяц. Дата начала: {sortedPeriods.First().StartDate} > Дата начала месяца: {startMonth}");
            return (false, errors);
        }

        if (sortedPeriods.Last().EndDate != endMonth)
        {
            errors.Add($"Период не покрывает месяц. Дата окончания: {sortedPeriods.Last().EndDate} > Дата окончания месяца: {endMonth}");
            return (false, errors);
        }

        for (int i = 1; i  < sortedPeriods.Length; i++)
        {
            if (sortedPeriods[i].StartDate != sortedPeriods[i - 1].EndDate.AddDays(1))
            {
                errors.Add($"Между периодами есть разрыв. [Период {i + 1}] Дата начала должна быть {sortedPeriods[i - 1].EndDate.AddDays(1)}");
                return (false, errors);
            }
        }

        return (!errors.Any(), errors);
    }
}
