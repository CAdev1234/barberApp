
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Barber.ViewModels;
using Rg.Plugins.Popup.Pages;
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
    public partial class ClientPage : ContentPage
    {
        private PopupPage popupPage = new LoadPopupPage();
        private bool IsInitComponent = true;
        private List<User> m_lstClients = new List<User>();
        public ClientPage()
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
            drawPage();
        }
        private async void drawPage()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            ClientRequest res = await ApiService.GetOneWithGet<ClientRequest>("clients");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
            {
                return;
            }

            if (res.Success)
            {
                foreach (User item in res.Users)
                {
                    item.fullname = item.firstname + " " + item.lastname;
                    item.avatar = ApiService.SERVER_HOST + item.avatar;
                }
                if(res.Users.Count > 0)
                {
                    TitleBar.Text = res.Users.Count.ToString() + " CLIENTS";
                    Blast.IsVisible = true;
                    Clients.ItemsSource = res.Users;
                    m_lstClients = res.Users;
                    Clients.IsVisible = true;
                    InvitePan.IsVisible = false;
                }
                else
                {
                    InvitePan.IsVisible = true;
                }
            }
        }
        private async void itemClicked(Object sender, EventArgs e)
        {
            var stack = (StackLayout)sender;
            User client = (User)stack.BindingContext;
            await Navigation.PushAsync(new ClientDetailPage(client));
        }

        private async void announcement(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AnnouncementPage(m_lstClients));
        }

        private async void inviteClients(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "You should definitely check out this app, theCut. Me and other clients can book appointments with you 24/7 and pay you through the app. Plus, you can brand yourself to all the people in our city. https://book.thecut.co/invite-barber",
                Title = "Share With"
            });
        }
    }
}