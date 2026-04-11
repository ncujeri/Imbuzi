using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class HeatCycleService
{
    public record HeatPrediction(
        Guid DoeId,
        DateTime LastObservedHeat,
        DateTime NextExpectedHeat,
        int DaysUntilHeat,
        bool IsInHeatWindow
    );

    public HeatPrediction? PredictNextHeat(
        IEnumerable<HeatRecord> heatRecords,
        FarmSettings settings,
        DateTime? asOfDate = null)
    {
        if (!settings.EnableHeatTracking)
            return null;

        var today = asOfDate ?? DateTime.UtcNow.Date;

        var latest = heatRecords
            .OrderByDescending(h => h.ObservedDate)
            .FirstOrDefault();

        if (latest is null)
            return null;

        var nextHeat = latest.ObservedDate.AddDays(settings.HeatCycleDays);
        var daysUntil = (nextHeat - today).Days;

        return new HeatPrediction(
            DoeId: latest.AnimalId,
            LastObservedHeat: latest.ObservedDate,
            NextExpectedHeat: nextHeat,
            DaysUntilHeat: daysUntil,
            IsInHeatWindow: Math.Abs(daysUntil) <= 1
        );
    }

    public IReadOnlyList<HeatPrediction> GetUpcomingHeats(
        ILookup<Guid, HeatRecord> heatRecordsByDoe,
        FarmSettings settings,
        DateTime? asOfDate = null)
    {
        if (!settings.EnableHeatTracking)
            return Array.Empty<HeatPrediction>();

        var predictions = new List<HeatPrediction>();

        foreach (var group in heatRecordsByDoe)
        {
            var prediction = PredictNextHeat(group, settings, asOfDate);
            if (prediction is not null && prediction.DaysUntilHeat <= settings.HeatAlertDaysBefore)
            {
                predictions.Add(prediction);
            }
        }

        return predictions.OrderBy(p => p.DaysUntilHeat).ToList();
    }
}
