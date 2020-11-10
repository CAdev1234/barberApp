using Android.Widget;
using Barber.Droid.Services;
using Barber.Services;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToastImpl))]
namespace Barber.Droid.Services {
    public class ToastImpl : IToast {

        public void Show(string msg, bool longShow = true) {
            try {
                Device.BeginInvokeOnMainThread(() => {
                    var toast = Toast.MakeText(Forms.Context, msg, longShow ? ToastLength.Long : ToastLength.Short);
                    toast.Show();
                    toast.Dispose();
                });
            }
            catch (Exception) {

            }
        }

    }
}