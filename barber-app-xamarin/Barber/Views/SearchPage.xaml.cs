using Barber.Models;
using Barber.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private SearchViewModel viewModelSearch;
        private bool IsInitComponent = true;
        public SearchPage()
        {
            checkPermission();   
        }
        private async void checkPermission()
        {
            var status = await CheckAndRequestPermissionAsync(new Permissions.LocationWhenInUse());
            if (status != PermissionStatus.Granted)
            {
                return;
            }
        }
        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }
        protected override void OnAppearing()
        {
            if(IsInitComponent)
            {
                InitializeComponent();
                IsInitComponent = false;
            }
            base.OnAppearing();

            SearchData newData = new SearchData();
            newData.location = "";
            newData.name = "";
            BindingContext = viewModelSearch = new SearchViewModel(newData);
        }

        private async void barberItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;
            User user = (User)gridItem.BindingContext;
            user.fullname = user.firstname + " " + user.lastname;

            await Navigation.PushAsync(new BarberViewPage(user));
        }

        private async void SearchButtonPressed(object sender, EventArgs e)
        {
            SearchData newData = new SearchData();
            newData.location = SearchLocation.Text;
            newData.name = SearchName.Text;

            await viewModelSearch.drawBarbersAsync(newData);
        }
    }
}