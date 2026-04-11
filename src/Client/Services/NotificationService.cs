using ImbuziSmart.Shared.Entities;
using Microsoft.JSInterop;

namespace ImbuziSmart.Client.Services;

public class NotificationService
{
    private readonly IJSRuntime _js;

    public NotificationService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        var result = await _js.InvokeAsync<string>("imbuziNotifications.requestPermission");
        return result == "granted";
    }

    public async Task SendAlertAsync(string title, string body, FarmSettings settings)
    {
        if (!settings.EnablePushNotifications)
            return;

        await _js.InvokeVoidAsync("imbuziNotifications.showNotification", title, body);
    }

    public async Task CheckAndNotifyAsync(
        IEnumerable<GestationTimerService.GestationStatus> gestations,
        IEnumerable<WithdrawalPeriodService.ActiveWithdrawal> withdrawals,
        IEnumerable<HeatCycleService.HeatPrediction> heatPredictions,
        FarmSettings settings)
    {
        if (!settings.EnablePushNotifications)
            return;

        if (settings.NotifyKiddingWatch)
        {
            foreach (var g in gestations.Where(g => g.IsOnKiddingWatch))
            {
                await SendAlertAsync(
                    "Kidding Watch",
                    $"Doe is due in {g.DaysRemaining} days!",
                    settings);
            }
        }

        if (settings.NotifyWithdrawalExpiry)
        {
            foreach (var w in withdrawals.Where(w => w.DaysRemaining <= 1))
            {
                await SendAlertAsync(
                    "Withdrawal Expiring",
                    $"{w.Medication} withdrawal ends tomorrow.",
                    settings);
            }
        }

        if (settings.NotifyHeatCycle)
        {
            foreach (var h in heatPredictions)
            {
                await SendAlertAsync(
                    "Heat Alert",
                    $"Doe approaching heat in {h.DaysUntilHeat} days — ready for buck.",
                    settings);
            }
        }
    }
}
