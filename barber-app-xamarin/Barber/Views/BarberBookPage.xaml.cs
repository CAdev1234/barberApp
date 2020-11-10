using Barber.Models;
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
    public partial class BarberBookPage : ContentPage
    {
        private BarberBookViewModel barberBook = null;
        private string currenttime;
        private bool IsInitComponent = true;
        public BarberBookPage()
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

            currenttime = DateTime.Now.ToString("yyyy-MM-dd");
            BindingContext = barberBook = new BarberBookViewModel(currenttime);

            MessagingCenter.Subscribe<BarberBookPage>(this, "RefreshBook", (sender) => {
                barberBook.drawBooks(currenttime);
            });
            BookDate.Date = DateTime.Now;
        }

        private async void createEvent(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BarberCreateEventPage());
        }
        private async void filterCalendar(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BarberFilterPage());
        }
        private async void request(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BarberRequestPage());
        }

        private void dateSelected(object sender, DateChangedEventArgs e)
        {
            var datepicker = (DatePicker)sender;
            string time = datepicker.Date.ToString("yyyy-MM-dd");
            if (barberBook == null)
                return;
            currenttime = time;
            barberBook.drawBooks(time);
        }

        private async void bookItemClicked(object sender, EventArgs e)
        {
            var grid = (StackLayout)sender;
            Book book = (Book)grid.BindingContext;
            await Navigation.PushAsync(new BookDetailPage(book));
        }
    }
}