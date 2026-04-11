namespace ImbuziSmart.Shared.Entities;

public class MedicalLog : BaseEntity
{
    public Guid AnimalId { get; set; }
    public DateTime TreatmentDate { get; set; }
    public string Medication { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public int? WithdrawalPeriodDays { get; set; }
    public DateTime? WithdrawalEndDate =>
        WithdrawalPeriodDays.HasValue
            ? TreatmentDate.AddDays(WithdrawalPeriodDays.Value)
            : null;
    public string? VetName { get; set; }
    public string? Notes { get; set; }
}
