using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class GestationTimerService
{
    public const int GestationDays = 150;
    public const int KiddingWatchDays = 145;
    public const int WeaningDaysPostBirth = 90;

    public record GestationStatus(
        Guid MatingRecordId,
        Guid DoeId,
        DateTime MatingDate,
        DateTime ExpectedKiddingDate,
        int DaysRemaining,
        bool IsOnKiddingWatch,
        bool IsOverdue,
        DateTime? WeaningDate,
        bool IsWeaningDue
    );

    public GestationStatus GetStatus(MatingRecord record, DateTime? asOfDate = null)
    {
        var today = asOfDate ?? DateTime.UtcNow.Date;
        var expected = record.MatingDate.AddDays(GestationDays);
        var kiddingWatchStart = record.MatingDate.AddDays(KiddingWatchDays);
        var daysRemaining = (expected - today).Days;

        DateTime? weaningDate = null;
        bool isWeaningDue = false;

        if (record.ActualKiddingDate.HasValue)
        {
            weaningDate = record.ActualKiddingDate.Value.AddDays(WeaningDaysPostBirth);
            isWeaningDue = today >= weaningDate;
        }

        return new GestationStatus(
            MatingRecordId: record.Id,
            DoeId: record.DoeId,
            MatingDate: record.MatingDate,
            ExpectedKiddingDate: expected,
            DaysRemaining: Math.Max(daysRemaining, 0),
            IsOnKiddingWatch: today >= kiddingWatchStart && record.ActualKiddingDate is null,
            IsOverdue: today > expected && record.ActualKiddingDate is null,
            WeaningDate: weaningDate,
            IsWeaningDue: isWeaningDue
        );
    }

    public IReadOnlyList<GestationStatus> GetActiveGestations(
        IEnumerable<MatingRecord> records, DateTime? asOfDate = null)
    {
        return records
            .Where(r => r.ActualKiddingDate is null)
            .Select(r => GetStatus(r, asOfDate))
            .OrderBy(s => s.DaysRemaining)
            .ToList();
    }

    public IReadOnlyList<GestationStatus> GetWeaningAlerts(
        IEnumerable<MatingRecord> records, DateTime? asOfDate = null)
    {
        return records
            .Where(r => r.ActualKiddingDate.HasValue)
            .Select(r => GetStatus(r, asOfDate))
            .Where(s => s.IsWeaningDue)
            .ToList();
    }
}
