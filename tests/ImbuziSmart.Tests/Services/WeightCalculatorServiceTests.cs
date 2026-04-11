using ImbuziSmart.Client.Services;

namespace ImbuziSmart.Tests.Services;

public class WeightCalculatorServiceTests
{
    private readonly WeightCalculatorService _sut = new();

    [Fact]
    public void CalculateFromHeartGirth_ValidInputs_ReturnsCorrectWeight()
    {
        // Girth=70cm, Length=65cm → (70² × 65) / 10838 = 318500 / 10838 ≈ 29.39 kg
        var result = _sut.CalculateFromHeartGirth(70, 65);

        Assert.Equal(29.39, result.Kilograms);
        Assert.Equal("HeartGirth", result.Method);
    }

    [Fact]
    public void CalculateFromHeartGirth_SmallGoat_ReturnsCorrectWeight()
    {
        // Girth=50cm, Length=40cm → (2500 × 40) / 10838 ≈ 9.23 kg
        var result = _sut.CalculateFromHeartGirth(50, 40);

        Assert.Equal(9.23, result.Kilograms);
        Assert.Equal("HeartGirth", result.Method);
    }

    [Fact]
    public void CalculateFromHeartGirth_LargeGoat_ReturnsCorrectWeight()
    {
        // Girth=90cm, Length=80cm → (8100 × 80) / 10838 ≈ 59.79 kg
        var result = _sut.CalculateFromHeartGirth(90, 80);

        Assert.Equal(59.79, result.Kilograms);
        Assert.Equal("HeartGirth", result.Method);
    }

    [Fact]
    public void CalculateFromHeartGirth_ZeroGirth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.CalculateFromHeartGirth(0, 65));
    }

    [Fact]
    public void CalculateFromHeartGirth_NegativeGirth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.CalculateFromHeartGirth(-10, 65));
    }

    [Fact]
    public void CalculateFromHeartGirth_ZeroLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.CalculateFromHeartGirth(70, 0));
    }

    [Fact]
    public void CalculateFromHeartGirth_NegativeLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.CalculateFromHeartGirth(70, -5));
    }

    [Fact]
    public void RecordScaleWeight_ValidInput_ReturnsWeight()
    {
        var result = _sut.RecordScaleWeight(35.5);

        Assert.Equal(35.5, result.Kilograms);
        Assert.Equal("Scale", result.Method);
    }

    [Fact]
    public void RecordScaleWeight_ZeroWeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.RecordScaleWeight(0));
    }

    [Fact]
    public void RecordScaleWeight_NegativeWeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.RecordScaleWeight(-10));
    }
}
