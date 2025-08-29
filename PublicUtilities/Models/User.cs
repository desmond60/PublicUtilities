namespace PublicUtilities.Models;

public class User
{
    public int Id { get; set; }
    public string PersonalAccount { get; set; }

    public ICollection<ResidentPeriod> ResidentPeriods { get; set; } = new List<ResidentPeriod>();
    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
    public ICollection<CalculationResult> СalculationResults { get; set; } = new List<CalculationResult>();
}
