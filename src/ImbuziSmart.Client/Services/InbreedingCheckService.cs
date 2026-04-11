using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Client.Services;

public class InbreedingCheckService
{
    public record InbreedingResult(bool IsAllowed, string? Reason = null);

    public InbreedingResult CheckMatingAllowed(Animal buck, Animal doe, FarmSettings settings)
    {
        if (!settings.EnableInbreedingCheck)
            return new InbreedingResult(true);

        if (buck.SireId.HasValue && doe.SireId.HasValue && buck.SireId == doe.SireId)
            return new InbreedingResult(false, "Buck and Doe share the same Sire (father).");

        if (buck.DamId.HasValue && doe.DamId.HasValue && buck.DamId == doe.DamId)
            return new InbreedingResult(false, "Buck and Doe share the same Dam (mother).");

        if (buck.Id == doe.SireId)
            return new InbreedingResult(false, "Buck is the Doe's father.");

        if (doe.Id == buck.DamId)
            return new InbreedingResult(false, "Doe is the Buck's mother.");

        return new InbreedingResult(true);
    }
}
