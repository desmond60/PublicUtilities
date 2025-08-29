namespace PublicUtilities.Models;

public class Tariff
{
    public int Id { get; set; }
    public string ServiceType { get; set; }
    public string? Component { get; set; }
    public double Price { get; set; }
    public double? Normative { get; set; }
    public string Unit { get; set; }
}
