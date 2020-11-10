using Barber.Models;
using Barber.Services;
using Barber.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        private bool IsInitComponent = true;
        public NotificationPage()
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

            BindingContext = new NotificationViewModel();

            MessagingCenter.Subscribe<NotificationPage>(this, "RefreshView", (sender) =>
            {
                BindingContext = new NotificationViewModel();
            });
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            await ApiService.GetOneWithGet<SuccessRequest>("showNotification");
        }
        public async void itemClicked(object sender, EventArgs e)
        {
            var stack = (StackLayout)sender;
            Notification item = (Notification)stack.BindingContext;
            if (item.type == 0 || item.type == 3)
            {
                await Navigation.PushAsync(new BookDetailPage(item.book));
            }
            else if(item.type == 1)
            {

            }
            else if(item.type == 2 && !SharedService.GetUser().User.barber)
            {
                await Navigation.PushAsync(new BarberViewPage(item.sender));
            }
            else if(item.type == 3)
            {

            }
        }
    }
}