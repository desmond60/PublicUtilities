using System.Text.Json.Serialization;

namespace PublicUtilities.Models;

public class CalculationResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string ServiceType { get; set; } // ХВС ГВС ЭЭ
    public string? Component { get; set; } // ГВС (ТН, ТЭ), для ЭЭ (День, Ночь)
    public double Volume { get; set; } // V
    public double Tariff { get; set; } // T
    public double Price { get; set; } // P = V*T

    [JsonIgnore]
    public User User { get; set; }
}
