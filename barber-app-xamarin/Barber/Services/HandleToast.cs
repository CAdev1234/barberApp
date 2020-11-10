using Xamarin.Forms;

namespace Barber.Services
{
    class HandleToast
    {
        public static void Toast(string message, bool longshow = true)
        {
            DependencyService.Get<IToast>().Show(message, longshow);
        }
    }
}