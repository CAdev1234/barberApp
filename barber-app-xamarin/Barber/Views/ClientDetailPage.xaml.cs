
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
    public partial class ClientDetailPage : ContentPage
    {
        private User currentClient = new User();
        private ClientBlockRequest curretClientBlock = new ClientBlockRequest();
        private PopupPage popupPage = new LoadPopupPage();
        public ClientDetailPage(User client)
        {
            InitializeComponent();
            
            currentClient = client;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Avatar.Source = currentClient.avatar;
            FullName.Text = currentClient.fullname;

            drawPage();
        }
        private async void drawPage()
        {
            ClientDetailRequest res = await ApiService.GetOneWithPost<ClientDetailRequest, User>(currentClient, "clientdetail");
            if (res == null)
                return;

            if(res.Success)
            {
                List<Book> lstUpTmp = new List<Book>();
                List<Book> lstPastTmp = new List<Book>();
                foreach (Book book in res.Books)
                {
                    book.client.fullname = book.client.firstname + " " + book.client.lastname;
                    book.allservicename = book.services[0].name;
                    for (int i = 1; i < book.services.Count; i++)
                        book.allservicename += " + " + book.services[1].name;

                    if (book.state == 0)
                    {
                        book.statename = "REQUESTED";
                        book.stateColor = "#ddc686";
                    }
                    else if (book.state == 1)
                    {
                        book.statename = "CANCELLED";
                        book.stateColor = "#db6e53";
                    }
                    else if (book.state == 2)
                    {
                        book.statename = "CONFIRMED";
                        book.stateColor = "#0dd6f7";
                    }
                    else if (book.state == 3)
                    {
                        book.statename = "COMPLETED";
                        book.stateColor = "#51f70d";
                    }
                    else if (book.state == 4)
                    {
                        book.statename = "DECLINED";
                        book.stateColor = "#db6e53";
                    }

                    book.barber.avatar = ApiService.SERVER_HOST + book.barber.avatar;
                    book.barber.fullname = book.barber.firstname + book.barber.lastname;
                    book.client.avatar = ApiService.SERVER_HOST + book.client.avatar;
                    book.client.fullname = book.client.firstname + book.client.lastname;

                    DateTime bookDate = DateTime.Parse(book.date);
                    book.date = bookDate.ToString("MM/dd/yyyy");

                    bookDate = Convert.ToDateTime(book.date);
                    int compare = DateTime.Compare(DateTime.Now, bookDate);
                    if (compare >= 0)
                        lstPastTmp.Add(book);
                    else
                        lstUpTmp.Add(book);
                }
                Upcoming.ItemsSource = lstUpTmp;
                Past.ItemsSource = lstPastTmp;
                if (lstUpTmp.Count > 0)
                    UpcomingPan.IsVisible = true;
                if (lstPastTmp.Count > 0)
                    PastPan.IsVisible = true;

                curretClientBlock.id = currentClient.id;
                curretClientBlock.block = res.FavBarber.Block;

                if(curretClientBlock.block == 0)
                    lblBlock.Text = "BLOCK";
                else
                    lblBlock.Text = "UNBLOCK";
            }
        }

        private async void bookItemClicked(object sender, EventArgs e)
        {
            var stacklayout = (StackLayout)sender;
            Book selectBook = (Book)stacklayout.BindingContext;
            await Navigation.PushAsync(new BookDetailPage(selectBook));
        }

        private async void blockClicked(object sender, EventArgs e)
        {
            string message = "Are you sure you want to block this client? They will be no longer be able to appointment with you.";
            if(curretClientBlock.block == 1)
                message = "Are you sure you want to unblock this client? They will be able to appointment with you.";

            bool answer = await DisplayAlert("Are you sure?", message, "Yes", "Cancel");
            if (!answer)
                return;

            curretClientBlock.block = curretClientBlock.block == 1 ? 0 : 1;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, ClientBlockRequest>(curretClientBlock, "blockclient");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if(res.Success)
            {
                if(curretClientBlock.block == 0)
                {
                    lblBlock.Text = "BLOCK";
                    HandleToast.Toast("This client unblocked successfully.");
                }
                else
                {
                    lblBlock.Text = "UNBLOCK";
                    HandleToast.Toast("This client blocked successfully.");
                }
            }
        }

        private async void deleteClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Are you sure?", "Are you sure you want to remove this client?", "Yes", "Cancel");
            if (!answer)
                return;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, User>(currentClient, "deleteclient");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if(res.Success)
            {
                HandleToast.Toast("This client removed successfully.");
            }
        }
    }
}