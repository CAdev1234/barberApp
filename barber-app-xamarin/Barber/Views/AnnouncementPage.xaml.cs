
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnnouncementPage : ContentPage
    {
        public List<User> AllClients = new List<User>();
        private PopupPage popupPage = new LoadPopupPage();
        public AnnouncementPage(List<User> clients)
        {
            InitializeComponent();
            AllClients = clients;
        }

        public async void send(object sender, EventArgs e)
        {
            if(Announcement.Text == null || Announcement.Text == "")
            {
                HandleToast.Toast("Invalid announcement");
                return;
            }
            Announcement announcement = new Announcement();
            announcement.Users = AllClients;
            announcement.content = Announcement.Text;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, Announcement>(announcement, "broadcast");

            if (res == null)
                return;

            if (res.Success)
            {
                HandleToast.Toast("Success");
                Navigation.PopAsync(true);
            }
        }
    }
}