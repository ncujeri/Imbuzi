using ImbuziSmart.Shared.ValueObjects;

namespace ImbuziSmart.Client.Services;

public class WeightCalculatorService
{
    private const double HeartGirthDivisor = 10838.0;

    /// <summary>
    /// Estimates goat weight using the Heart Girth method.
    /// Formula: Weight(kg) = (HeartGirth_cm² x BodyLength_cm) / 10838
    /// </summary>
    public Weight CalculateFromHeartGirth(double heartGirthCm, double bodyLengthCm)
    {
        if (heartGirthCm <= 0)
            throw new ArgumentOutOfRangeException(nameof(heartGirthCm), "Heart girth must be positive.");
        if (bodyLengthCm <= 0)
            throw new ArgumentOutOfRangeException(nameof(bodyLengthCm), "Body length must be positive.");

        double weightKg = (heartGirthCm * heartGirthCm * bodyLengthCm) / HeartGirthDivisor;

        return new Weight(
            Kilograms: Math.Round(weightKg, 2),
            MeasuredAt: DateTime.UtcNow,
            Method: "HeartGirth"
        );
    }

    /// <summary>
    /// Records a direct scale measurement.
    /// </summary>
    public Weight RecordScaleWeight(double kilograms)
    {
        if (kilograms <= 0)
            throw new ArgumentOutOfRangeException(nameof(kilograms), "Weight must be positive.");

        return new Weight(
            Kilograms: Math.Round(kilograms, 2),
            MeasuredAt: DateTime.UtcNow,
            Method: "Scale"
        );
    }
}
