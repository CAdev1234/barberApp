
using Barber.Models;
using Barber.Services;
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
    public partial class CreateServicePage : ContentPage
    {
        private bool bEdit = false;
        private Service currentService = new Service();
        private string hour = "";
        private string min = "";
        private string[] lstMinutes = new string[12];
        private string[] lstHours = new string[24];
        public CreateServicePage(Service service)
        {
            InitializeComponent();

            for (int i = 0; i < 12; i++)
            {
                lstMinutes[i] = i * 5 + " minutes";
            }
            for (int i = 0; i < 24; i++)
            {
                lstHours[i] = i + " hour";
            }
            pkHour.ItemsSource = lstHours;
            pkMin.ItemsSource = lstMinutes;

            if (service != null)
            {
                currentService = service;

                bEdit = true;
                edtName.Text = service.name;
                edtPrice.Text = service.price.ToString();
                if (service.hour != null)
                    pkHour.SelectedIndex = int.Parse(service.hour);
                if(service.min != null)
                    pkMin.SelectedIndex = int.Parse(service.min)/5;
                edtDescription.Text = service.description;
            }
        }
        private async void save(object sender, EventArgs e)
        {
            string name = edtName.Text;
            string price = edtPrice.Text;
            string descript = edtDescription.Text;
            if(name == null || name == "")
            {
                HandleToast.Toast("Invalid name.");
                return;
            }
            if (price == null || price == "")
            {
                HandleToast.Toast("Invalid price.");
                return;
            }
            if (hour == "" && min == "")
            {
                HandleToast.Toast("Invalid duration.");
                return;
            }
            if (descript == null || descript == "")
            {
                HandleToast.Toast("Invalid description.");
                return;
            }
            Service service = new Service();
            service.name = name;
            service.price = int.Parse(price);
            service.hour = hour;
            service.min = min;
            service.description = descript;
            string url = "createservice";
            if (bEdit)
            {
                url = "editservice";
                service.id = currentService.id;
            }
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, Service>(service, url);
            if (res == null)
                return;

            if (res.Success)
            {
                MessagingCenter.Send<BarberHomePage>(new BarberHomePage(), "RefreshService");
                Navigation.PopAsync(true);
            }
        }

        private void hourSelected(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            hour = (picker.SelectedIndex).ToString();
            if (hour == "0")
                hour = "";
        }

        private void minSelected(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            min = ((picker.SelectedIndex) * 5).ToString();
            if (min == "0")
                min = "";
        }
    }
}