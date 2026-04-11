using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Shared.Entities;

public class CostEntry : BaseEntity
{
    public Guid? AnimalId { get; set; }
    public CostCategory Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
