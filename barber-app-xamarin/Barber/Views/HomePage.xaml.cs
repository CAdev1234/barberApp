
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Barber.ViewModels;
using Newtonsoft.Json;
using Plugin.Media;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        private PopupPage popupPage = new LoadPopupPage();
        private HomeViewModel homepagemodel;
        private int m_newAlarm = 0;
        public HomePage()
		{
			InitializeComponent();
			BindingContext = homepagemodel = new HomeViewModel();

            drawProfile();
            drawNotification();

            MessagingCenter.Subscribe<HomePage>(this, "RefreshProfile", (sender) => {
                drawProfile();
            });

            MessagingCenter.Subscribe<HomePage>(this, "RefreshFavouriteBarber", (sender) => {
                homepagemodel.drawBarbersAsync();
            });

            MessagingCenter.Subscribe<HomePage>(this, "RefreshNews", (sender) => {
                drawNotification();
            });

            MessagingCenter.Subscribe<HomePage>(this, "RefreshNews", (sender) => {
                drawNotification();
            });

            MessagingCenter.Subscribe<Object>(this, "NewNotification", (sender) => {
                drawNewAlarm();
            });
        }
        private async void drawNewAlarm()
        {
            m_newAlarm ++;
            await Task.Run(() => {
                Device.InvokeOnMainThreadAsync(() => {
                    NewBadge.BadgeText = m_newAlarm.ToString();
                });
            });
        }
        private async void drawNotification()
        {
            NewAlarmRequest res = await ApiService.GetOneWithGet<NewAlarmRequest>("newalarm");
            if (res == null)
                return;

            if (res.Success)
            {
                NewBadge.BadgeText = res.NewAlarm.ToString();
                m_newAlarm = res.NewAlarm;
            }
        }
        private async void drawProfile()
        {
            string fullname = SharedService.GetUser().User.fullname;
            if (fullname != "")
                FullName.Text = fullname;
            string avatar = SharedService.GetUser().User.avatar;
            if (avatar != "")
                Avatar.Source = avatar;
            
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ContentPage());
		}

        private async void barberItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;
            User user = (User)gridItem.BindingContext;

            await Navigation.PushAsync(new BarberViewPage(user, true));
        }

        private async void GoCutViewPage(object sender, EventArgs e)
        {
            List<Cut> MyCuts = homepagemodel.getMyCuts();
            var cut = ((ImageButton)sender).BindingContext;
            Cut selectedCut = (Cut)cut;
            if(selectedCut.id == -1)
                addCutPhoto(sender, e);
            else
            {
                int position = MyCuts.IndexOf(selectedCut);
                await Navigation.PushAsync(new CutViewPage(MyCuts, position));
            }
        }
        private async void GoSearchPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage());
        }

        private async void GoNotificatinPage(object sender, EventArgs e)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            NewBadge.BadgeText = "0";
            m_newAlarm = 0;
            await Navigation.PushAsync(new NotificationPage());
        }

        private async void addCutPhoto(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();
                homepagemodel.addCutPhoto(file.Path);
            }
            catch (Exception ex)
            {
                string erro = ex.Message;
            }
        }

        private async void editProfile(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountPage("Edit Account"));
        }

        private async void book_Clicked(object sender, EventArgs e)
        {
            var btnItem = (Button)sender;
            StackLayout gridItem = (StackLayout)btnItem.Parent;
            User barber = (User)gridItem.BindingContext;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            ServiceRequest res = await ApiService.GetOneWithPost<ServiceRequest, User>(barber, "service");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                if (res.Services.Count == 0)
                {
                    HandleToast.Toast("This barber does not have any service.");
                    return;
                }

                foreach (Service item in res.Services)
                {
                    item.image = "uncheck.png";
                    item.check = "0";
                }
                await Navigation.PushAsync(new BookingPage(barber, null, res.Services));
            }
        }
    }
}