using ImbuziSmart.Shared.Enums;
using ImbuziSmart.Shared.ValueObjects;

namespace ImbuziSmart.Shared.Entities;

public class Animal : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public Species Species { get; set; } = Species.Goat;
    public string? Breed { get; set; }
    public Sex Sex { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Guid? SireId { get; set; }
    public Guid? DamId { get; set; }
    public AnimalStatus Status { get; set; } = AnimalStatus.Active;
    public List<Photo> Photos { get; set; } = new();
    public List<Weight> WeightHistory { get; set; } = new();
    public string? Notes { get; set; }
}
