namespace ImbuziSmart.Shared.Entities;

public class FarmSettings : BaseEntity
{
    public bool EnableInbreedingCheck { get; set; } = true;
    public bool EnableHeatTracking { get; set; } = true;
    public int HeatCycleDays { get; set; } = 21;
    public bool EnablePushNotifications { get; set; }
    public bool NotifyKiddingWatch { get; set; } = true;
    public bool NotifyWeaning { get; set; } = true;
    public bool NotifyWithdrawalExpiry { get; set; } = true;
    public bool NotifyHeatCycle { get; set; } = true;
    public int KiddingWatchDaysBefore { get; set; } = 5;
    public int HeatAlertDaysBefore { get; set; } = 2;
    public string Currency { get; set; } = "ZAR";
}
