using System.Text.Json.Serialization;

namespace PublicUtilities.Models;

public class Metric
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public double? ColdWater { get; set; }
    public double? HotWater { get; set; }
    public double? ElectricityDay { get; set; }
    public double? ElectricityNight { get; set; }

    [JsonIgnore]
    public User User { get; set; }
}
