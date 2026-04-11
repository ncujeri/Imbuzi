namespace ImbuziSmart.Shared.Entities;

public class FarmSettings : BaseEntity
{
    // Inbreeding
    public bool EnableInbreedingCheck { get; set; } = true;

    // Heat cycle tracking
    public bool EnableHeatTracking { get; set; } = true;
    public int HeatCycleDays { get; set; } = 21;

    // Push notifications (opt-in)
    public bool EnablePushNotifications { get; set; }

    // Alert toggles
    public bool NotifyKiddingWatch { get; set; } = true;
    public bool NotifyWeaning { get; set; } = true;
    public bool NotifyWithdrawalExpiry { get; set; } = true;
    public bool NotifyHeatCycle { get; set; } = true;

    // Alert timing
    public int KiddingWatchDaysBefore { get; set; } = 5;
    public int HeatAlertDaysBefore { get; set; } = 2;

    // Regional
    public string Currency { get; set; } = "ZAR";
}
