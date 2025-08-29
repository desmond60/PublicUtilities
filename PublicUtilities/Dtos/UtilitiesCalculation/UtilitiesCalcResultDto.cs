namespace PublicUtilities.Dtos.UtilitiesCalculation;

public struct MetricCalculated
{
    public string ServiceType { get; set; } // ХВС ГВС ЭЭ
    public string? Component { get; set; } // ГВС (ТН, ТЭ), для ЭЭ (День, Ночь)
    public double Volume { get; set; } // V
    public double Tariff { get; set; } // T
    public double Price { get; set; } // P = V*T
}

public class UtilitiesCalcResultDto
{
    public bool IsCompleted { get; set; }
    public string Message { get; set; }
    public ICollection<string> Errors { get; set; }
    public ICollection<MetricCalculated> Metrics { get; set; }
    public double TotalSum { get; set; }
}
