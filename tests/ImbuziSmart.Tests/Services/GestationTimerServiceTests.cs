using ImbuziSmart.Client.Services;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Tests.Services;

public class GestationTimerServiceTests
{
    private readonly GestationTimerService _sut = new();

    private static MatingRecord CreateMatingRecord(DateTime matingDate, DateTime? actualKiddingDate = null)
    {
        return new MatingRecord
        {
            Id = Guid.NewGuid(),
            BuckId = Guid.NewGuid(),
            DoeId = Guid.NewGuid(),
            MatingDate = matingDate,
            ActualKiddingDate = actualKiddingDate
        };
    }

    [Fact]
    public void GetStatus_AtDay145_IsOnKiddingWatch()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate);
        var checkDate = matingDate.AddDays(145);

        var status = _sut.GetStatus(record, checkDate);

        Assert.True(status.IsOnKiddingWatch);
        Assert.False(status.IsOverdue);
        Assert.Equal(5, status.DaysRemaining);
    }

    [Fact]
    public void GetStatus_AtDay144_NotOnKiddingWatch()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate);
        var checkDate = matingDate.AddDays(144);

        var status = _sut.GetStatus(record, checkDate);

        Assert.False(status.IsOnKiddingWatch);
        Assert.False(status.IsOverdue);
        Assert.Equal(6, status.DaysRemaining);
    }

    [Fact]
    public void GetStatus_AtDay151_IsOverdue()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate);
        var checkDate = matingDate.AddDays(151);

        var status = _sut.GetStatus(record, checkDate);

        Assert.True(status.IsOverdue);
        Assert.True(status.IsOnKiddingWatch);
        Assert.Equal(0, status.DaysRemaining);
    }

    [Fact]
    public void GetStatus_AtDay150_NotYetOverdue()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate);
        var checkDate = matingDate.AddDays(150);

        var status = _sut.GetStatus(record, checkDate);

        Assert.False(status.IsOverdue);
        Assert.True(status.IsOnKiddingWatch);
        Assert.Equal(0, status.DaysRemaining);
    }

    [Fact]
    public void GetStatus_ExpectedKiddingDate_Is150DaysAfterMating()
    {
        var matingDate = new DateTime(2026, 3, 15);
        var record = CreateMatingRecord(matingDate);

        var status = _sut.GetStatus(record, matingDate);

        Assert.Equal(matingDate.AddDays(150), status.ExpectedKiddingDate);
        Assert.Equal(150, status.DaysRemaining);
    }

    [Fact]
    public void GetStatus_WithActualKiddingDate_NotOnWatch()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate, matingDate.AddDays(148));
        var checkDate = matingDate.AddDays(149);

        var status = _sut.GetStatus(record, checkDate);

        Assert.False(status.IsOnKiddingWatch);
        Assert.False(status.IsOverdue);
    }

    [Fact]
    public void GetStatus_WeaningAt90DaysPostBirth()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var kiddingDate = matingDate.AddDays(149);
        var record = CreateMatingRecord(matingDate, kiddingDate);
        var checkDate = kiddingDate.AddDays(90);

        var status = _sut.GetStatus(record, checkDate);

        Assert.True(status.IsWeaningDue);
        Assert.Equal(kiddingDate.AddDays(90), status.WeaningDate);
    }

    [Fact]
    public void GetStatus_WeaningNotYetDue()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var kiddingDate = matingDate.AddDays(149);
        var record = CreateMatingRecord(matingDate, kiddingDate);
        var checkDate = kiddingDate.AddDays(89);

        var status = _sut.GetStatus(record, checkDate);

        Assert.False(status.IsWeaningDue);
    }

    [Fact]
    public void GetStatus_NoKidding_NoWeaningDate()
    {
        var matingDate = new DateTime(2026, 1, 1);
        var record = CreateMatingRecord(matingDate);

        var status = _sut.GetStatus(record, matingDate.AddDays(10));

        Assert.Null(status.WeaningDate);
        Assert.False(status.IsWeaningDue);
    }

    [Fact]
    public void GetActiveGestations_ReturnsOnlyUnkidded_SortedByDaysRemaining()
    {
        var today = new DateTime(2026, 6, 1);
        var records = new[]
        {
            CreateMatingRecord(today.AddDays(-140)), // 10 days remaining
            CreateMatingRecord(today.AddDays(-100)), // 50 days remaining
            CreateMatingRecord(today.AddDays(-130), today.AddDays(-1)), // kidded — excluded
        };

        var result = _sut.GetActiveGestations(records, today);

        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].DaysRemaining);
        Assert.Equal(50, result[1].DaysRemaining);
    }

    [Fact]
    public void GetWeaningAlerts_ReturnsOnlyDueForWeaning()
    {
        var today = new DateTime(2026, 6, 1);
        var records = new[]
        {
            CreateMatingRecord(today.AddDays(-250), today.AddDays(-100)), // kidded 100 days ago → weaning due
            CreateMatingRecord(today.AddDays(-200), today.AddDays(-50)),  // kidded 50 days ago → not due yet
            CreateMatingRecord(today.AddDays(-100)),                       // not kidded → excluded
        };

        var result = _sut.GetWeaningAlerts(records, today);

        Assert.Single(result);
        Assert.True(result[0].IsWeaningDue);
    }
}
