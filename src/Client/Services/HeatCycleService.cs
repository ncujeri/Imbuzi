using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class HeatCycleService
{
    public record HeatPrediction(
        Guid AnimalId,
        DateTime LastObservedHeat,
        DateTime NextExpectedHeat,
        int DaysUntilHeat,
        bool IsInHeatWindow
    );

    public HeatPrediction? PredictNextHeat(
        IEnumerable<HeatRecord> heatRecords,
        Guid animalId,
        FarmSettings settings,
        DateTime? asOfDate = null)
    {
        if (!settings.EnableHeatTracking)
            return null;

        var latest = heatRecords
            .Where(h => h.AnimalId == animalId)
            .OrderByDescending(h => h.ObservedDate)
            .FirstOrDefault();

        if (latest is null)
            return null;

        var today = asOfDate ?? DateTime.UtcNow.Date;
        var nextHeat = latest.ObservedDate.AddDays(settings.HeatCycleDays);
        var daysUntil = (nextHeat - today).Days;

        // If the predicted date has passed, project forward to the next cycle
        while (daysUntil < -1)
        {
            nextHeat = nextHeat.AddDays(settings.HeatCycleDays);
            daysUntil = (nextHeat - today).Days;
        }

        return new HeatPrediction(
            AnimalId: animalId,
            LastObservedHeat: latest.ObservedDate,
            NextExpectedHeat: nextHeat,
            DaysUntilHeat: Math.Max(daysUntil, 0),
            IsInHeatWindow: Math.Abs(daysUntil) <= 1
        );
    }

    public IReadOnlyList<HeatPrediction> GetUpcomingHeats(
        IEnumerable<HeatRecord> allRecords,
        IEnumerable<Guid> doeIds,
        FarmSettings settings,
        DateTime? asOfDate = null)
    {
        if (!settings.EnableHeatTracking)
            return Array.Empty<HeatPrediction>();

        var predictions = new List<HeatPrediction>();

        foreach (var doeId in doeIds)
        {
            var prediction = PredictNextHeat(allRecords, doeId, settings, asOfDate);
            if (prediction is not null && prediction.DaysUntilHeat <= settings.HeatAlertDaysBefore)
            {
                predictions.Add(prediction);
            }
        }

        return predictions.OrderBy(p => p.DaysUntilHeat).ToList();
    }
}
