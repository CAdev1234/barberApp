using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private bool IsInitComponent = true;
        public SettingPage()
        {
        }
        protected override void OnAppearing()
        {
            if (IsInitComponent)
            {
                InitializeComponent();
                IsInitComponent = false;
            }
            base.OnAppearing();
        }

        [Obsolete]
        private async void settingItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;
            Label label = (Label)gridItem.Children[1];
            string title = label.Text;

            if(title == "Edit Account")
            {
                await Navigation.PushAsync(new AccountPage(title));
            }
            else if(title == "Change Password")
            {
                await Navigation.PushAsync(new AccountPage(title));
            }
            else if(title == "Payment Method")
            {
                await Navigation.PushAsync(new PaymentMethodPage());
            }
            else if (title == "Recommend theCut")
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = "You should definitely check out this app, theCut. See if your barber is on it, or find other barbers in your area. You can schedule your next haircut in advance, pay and tip all through the app. No more waiting in the shop. It's a game changer. https://book.thecut.co/client-recommend",
                    Title = "Share With"
                });
            }
            else if(title == "Invite Your Barber")
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = "You should definitely check out this app, theCut. Me and other clients can book appointments with you 24/7 and pay you through the app. Plus, you can brand yourself to all the people in our city. https://book.thecut.co/invite-barber",
                    Title = "Share With"
                });
            }
            else if (title == "Send Feedback")
            {
                await Navigation.PushAsync(new SupportFeedbackPage(SupportFeedbackPage.Feedback));
            }
            else if (title == "Support")
            {
                await Navigation.PushAsync(new SupportFeedbackPage(SupportFeedbackPage.Support));
            }
            else if (title == "Facebook")
            {
                Device.OpenUri(new Uri("http://www.facebook.com/thehaircutapp"));
            }
            else if (title == "Instagram")
            {
                Device.OpenUri(new Uri("http://www.instagram.com/thecutapp"));
            }
            else if (title == "Twitter")
            {
                Device.OpenUri(new Uri("http://www.twitter.com/thehaircutapp"));
            }
            else if (title == "Website")
            {
                Device.OpenUri(new Uri("http://www.thecut.co"));
            }
            else if (title == "Terms of Service")
            {
                Device.OpenUri(new Uri("https://www.thecut.co/terms-of-service"));
            }
            else if (title == "Privacy Policy")
            {
                Device.OpenUri(new Uri("https://www.thecut.co/privacy-policy"));
            }
            else if (title == "FAQs")
            {
                Device.OpenUri(new Uri("https://www.thecut.co/faqs"));
            }
            else if (title == "Visit theStore")
            {
                Device.OpenUri(new Uri("https://shop.thecut.co"));
            }
            else if (title == "Check for Updates")
            {
                Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=com.thecut.mobile.android.thecut"));
            }
            else if (title == "Rate theCut")
            {
                Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=com.thecut.mobile.android.thecut"));
            }
            else if (title == "Special Offers")
            {
                Device.OpenUri(new Uri("https://www.thecut.co/offers"));
            }
            else if (title == "Take a Survey")
            {
                Device.OpenUri(new Uri("https://www.thecut.co/clients/survey"));
            }
        }

        private void logout(object sender, EventArgs e)
        {
            LoginResponse res = new LoginResponse();
            res.Success = false;
            res.Token = "";
            res.User = new User { id = -1, avatar = "", barber = false, email = "", firstname = "", fullname = "", lastname = ""  };
            SharedService.SetUser(res);
            App.Current.MainPage = new AuthPage();
        }
    }
}