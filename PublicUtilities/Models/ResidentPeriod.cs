using System.Text.Json.Serialization;

namespace PublicUtilities.Models;

public class ResidentPeriod
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int ResidentCount { get; set; }

    [JsonIgnore]
    public User User { get; set; }
}
