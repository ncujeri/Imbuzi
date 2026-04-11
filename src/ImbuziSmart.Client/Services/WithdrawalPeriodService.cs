using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class WithdrawalPeriodService
{
    public record WithdrawalStatus(
        bool IsUnderWithdrawal,
        DateTime? LatestWithdrawalEnd,
        IReadOnlyList<ActiveWithdrawal> ActiveWithdrawals
    );

    public record ActiveWithdrawal(
        Guid MedicalLogId,
        string Medication,
        DateTime TreatmentDate,
        DateTime WithdrawalEndDate,
        int DaysRemaining
    );

    public WithdrawalStatus CheckWithdrawalStatus(
        IEnumerable<MedicalLog> medicalLogs, DateTime? asOfDate = null)
    {
        var today = asOfDate ?? DateTime.UtcNow.Date;

        var activeWithdrawals = medicalLogs
            .Where(m => m.WithdrawalEndDate.HasValue && m.WithdrawalEndDate.Value > today)
            .Select(m => new ActiveWithdrawal(
                MedicalLogId: m.Id,
                Medication: m.Medication,
                TreatmentDate: m.TreatmentDate,
                WithdrawalEndDate: m.WithdrawalEndDate!.Value,
                DaysRemaining: (m.WithdrawalEndDate!.Value - today).Days
            ))
            .OrderByDescending(a => a.WithdrawalEndDate)
            .ToList();

        return new WithdrawalStatus(
            IsUnderWithdrawal: activeWithdrawals.Count > 0,
            LatestWithdrawalEnd: activeWithdrawals.FirstOrDefault()?.WithdrawalEndDate,
            ActiveWithdrawals: activeWithdrawals
        );
    }

    public bool IsMarketReady(IEnumerable<MedicalLog> medicalLogs, DateTime? asOfDate = null)
    {
        return !CheckWithdrawalStatus(medicalLogs, asOfDate).IsUnderWithdrawal;
    }
}
