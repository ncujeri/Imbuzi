using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Shared.Entities;

public class HeatRecord : BaseEntity
{
    public Guid AnimalId { get; set; }
    public DateTime ObservedDate { get; set; }
    public string? Signs { get; set; }
    public HeatIntensity? Intensity { get; set; }
    public DateTime NextExpectedHeat => ObservedDate.AddDays(21);
}
