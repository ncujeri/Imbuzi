using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Client.Services;

public class ShadowLedgerService
{
    public record LedgerSummary(
        Guid? AnimalId,
        decimal TotalCost,
        Dictionary<CostCategory, decimal> CostsByCategory,
        int EntryCount
    );

    public LedgerSummary GetAnimalCosts(IEnumerable<CostEntry> costEntries, Guid animalId)
    {
        var entries = costEntries.Where(c => c.AnimalId == animalId).ToList();
        return BuildSummary(entries, animalId);
    }

    public LedgerSummary GetFarmWideCosts(IEnumerable<CostEntry> costEntries)
    {
        return BuildSummary(costEntries.ToList(), null);
    }

    public decimal CalculateBreakEven(decimal totalCosts, decimal marketPrice)
    {
        if (marketPrice <= 0)
            return 0;

        return totalCosts / marketPrice;
    }

    public decimal GetProfitOrLoss(decimal totalCosts, decimal salePrice)
    {
        return salePrice - totalCosts;
    }

    private static LedgerSummary BuildSummary(List<CostEntry> entries, Guid? animalId)
    {
        var byCategory = entries
            .GroupBy(c => c.Category)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.Amount));

        return new LedgerSummary(
            AnimalId: animalId,
            TotalCost: entries.Sum(c => c.Amount),
            CostsByCategory: byCategory,
            EntryCount: entries.Count
        );
    }
}
