using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class InbreedingCheckService
{
    public record InbreedingResult(bool IsAllowed, string? Reason = null);

    public InbreedingResult CheckMatingAllowed(Animal buck, Animal doe, FarmSettings settings)
    {
        if (!settings.EnableInbreedingCheck)
            return new InbreedingResult(true);

        // Check if they share a sire
        if (buck.SireId.HasValue && doe.SireId.HasValue && buck.SireId == doe.SireId)
            return new InbreedingResult(false, "Buck and Doe share the same sire (father).");

        // Check if they share a dam
        if (buck.DamId.HasValue && doe.DamId.HasValue && buck.DamId == doe.DamId)
            return new InbreedingResult(false, "Buck and Doe share the same dam (mother).");

        // Check if one is the parent of the other
        if (buck.Id == doe.SireId || buck.Id == doe.DamId)
            return new InbreedingResult(false, "Buck is a parent of the Doe.");

        if (doe.Id == buck.SireId || doe.Id == buck.DamId)
            return new InbreedingResult(false, "Doe is a parent of the Buck.");

        return new InbreedingResult(true);
    }
}
