using CommunityToolkit.WinUI.Notifications;

using Windows.UI.Notifications;

using WPFClient.Contracts.Services;

namespace WPFClient.Services;

public partial class ToastNotificationsService : IToastNotificationsService
{
    public ToastNotificationsService()
    {
    }

    public void ShowToastNotification(ToastNotification toastNotification)
    {
        ToastNotificationManagerCompat.CreateToastNotifier().Show(toastNotification);
    }
}
