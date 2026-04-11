namespace ImbuziSmart.Shared.Entities;

public class MatingRecord : BaseEntity
{
    public Guid BuckId { get; set; }
    public Guid DoeId { get; set; }
    public DateTime MatingDate { get; set; }
    public DateTime ExpectedKiddingDate => MatingDate.AddDays(150);
    public DateTime? ActualKiddingDate { get; set; }
    public string? Notes { get; set; }
}
