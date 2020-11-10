using Foundation;
using Barber.Services.iOS;
using Barber.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastImpl))]
namespace Barber.Services.iOS
{
    public class ToastImpl : IToast
    {
        const double LONG_DELAY = 3.5;
        const double SHORT_DELAY = 1;

        NSTimer alertDelay;
        UIAlertController alert;

        public void Show(string message, bool longShow = true)
        {
            ShowAlert(message, longShow ? LONG_DELAY : SHORT_DELAY);
        }

        void ShowAlert(string message, double seconds)
        {
            alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
            {
                dismissMessage();
            });
            alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }
        void dismissMessage()
        {
            if (alert != null)
            {
                alert.DismissViewController(true, null);
            }
            if (alertDelay != null)
            {
                alertDelay.Dispose();
            }
        }

    }
}