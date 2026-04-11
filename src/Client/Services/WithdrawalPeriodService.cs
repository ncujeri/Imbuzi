using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class WithdrawalPeriodService
{
    public record WithdrawalStatus(
        bool IsUnderWithdrawal,
        IReadOnlyList<ActiveWithdrawal> ActiveWithdrawals
    );

    public record ActiveWithdrawal(
        Guid MedicalLogId,
        string Medication,
        DateTime TreatmentDate,
        DateTime WithdrawalEndDate,
        int DaysRemaining
    );

    public WithdrawalStatus CheckAnimal(
        IEnumerable<MedicalLog> medicalLogs,
        Guid animalId,
        DateTime? asOfDate = null)
    {
        var today = asOfDate ?? DateTime.UtcNow.Date;

        var activeWithdrawals = medicalLogs
            .Where(m => m.AnimalId == animalId
                        && m.WithdrawalEndDate.HasValue
                        && m.WithdrawalEndDate.Value > today)
            .Select(m => new ActiveWithdrawal(
                MedicalLogId: m.Id,
                Medication: m.Medication,
                TreatmentDate: m.TreatmentDate,
                WithdrawalEndDate: m.WithdrawalEndDate!.Value,
                DaysRemaining: (m.WithdrawalEndDate!.Value - today).Days
            ))
            .OrderBy(a => a.DaysRemaining)
            .ToList();

        return new WithdrawalStatus(
            IsUnderWithdrawal: activeWithdrawals.Count > 0,
            ActiveWithdrawals: activeWithdrawals
        );
    }

    public bool CanMarkMarketReady(
        IEnumerable<MedicalLog> medicalLogs,
        Guid animalId,
        DateTime? asOfDate = null)
    {
        return !CheckAnimal(medicalLogs, animalId, asOfDate).IsUnderWithdrawal;
    }
}
