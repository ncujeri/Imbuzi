using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Client.Services;

public class ShadowLedgerService
{
    public record LedgerSummary(
        Guid AnimalId,
        decimal TotalCost,
        Dictionary<CostCategory, decimal> CostsByCategory
    );

    public LedgerSummary GetAnimalLedger(IEnumerable<CostEntry> allCosts, Guid animalId)
    {
        var animalCosts = allCosts
            .Where(c => c.AnimalId == animalId)
            .ToList();

        var byCategory = animalCosts
            .GroupBy(c => c.Category)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.Amount));

        return new LedgerSummary(
            AnimalId: animalId,
            TotalCost: animalCosts.Sum(c => c.Amount),
            CostsByCategory: byCategory
        );
    }

    public decimal CalculateBreakEven(IEnumerable<CostEntry> allCosts, Guid animalId)
    {
        return allCosts
            .Where(c => c.AnimalId == animalId)
            .Sum(c => c.Amount);
    }
}
