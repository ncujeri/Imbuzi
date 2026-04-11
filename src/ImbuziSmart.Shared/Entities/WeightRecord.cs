namespace ImbuziSmart.Shared.Entities;

public class WeightRecord : BaseEntity
{
    public Guid AnimalId { get; set; }
    public DateTime WeighedAt { get; set; }
    public double Kilograms { get; set; }

    /// <summary>"Scale" or "HeartGirth"</summary>
    public string Method { get; set; } = "Scale";

    /// <summary>Heart girth circumference in cm — populated when Method is HeartGirth.</summary>
    public double? HeartGirthCm { get; set; }

    /// <summary>Body length in cm — populated when Method is HeartGirth.</summary>
    public double? BodyLengthCm { get; set; }

    public string? Notes { get; set; }
}
