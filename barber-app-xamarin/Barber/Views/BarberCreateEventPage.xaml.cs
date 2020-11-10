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
    public partial class BarberCreateEventPage : ContentPage
    {
        private List<Service> m_lstServices;
        private List<Service> lstCurrentServices = new List<Service>();
        private bool bTimeOff = false;
        private int currentIndex = -1;
        private BookSetting currentSetting = new BookSetting();
        private string selectedTime = "";
        private PopupPage popupPage = new LoadPopupPage();
        public BarberCreateEventPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            currentSetting.barberid = SharedService.GetUser().User.id;
            currentSetting.auto_confirm = 0;
            currentSetting.auto_comment = null;
            currentSetting.multi_service = 0;
            currentSetting.require_phone = 0;

            getSetting();
            drawService();

            selectServicePan.IsVisible = false;
            titlePan.IsVisible = false;
            appTimePan.IsVisible = true;
            offTimePan.IsVisible = false;
        }
        private async void getSetting()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookSettingRequest res = await ApiService.GetOneWithGet<BookSettingRequest>("getbooksetting");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                currentSetting = res.BookSetting;
            }
        }
        private async void drawService()
        {
            ServiceRequest res = await ApiService.GetOneWithPost<ServiceRequest, User>(SharedService.GetUser().User, "service");
            if(res == null)
            {
                HandleToast.Toast("Please add service.");
                return;
            }
            foreach(Service item in res.Services)
            {
                item.image = "uncheck.png";
                item.check = "0";
            }
            selectServicePan.IsVisible = true;

            servicePan.HeightRequest = 60 * res.Services.Count;
            Services.ItemsSource = res.Services;
            m_lstServices = res.Services;
        }

        private async void appointment(object sender, EventArgs e)
        {
            appointmentimg.Source = "check.png";
            timeoffimg.Source = "uncheck.png";
            selectServicePan.IsVisible = true;
            titlePan.IsVisible = false;
            appTimePan.IsVisible = true;
            offTimePan.IsVisible = false;

            bTimeOff = false;
        }
        private async void timeoff(object sender, EventArgs e)
        {
            appointmentimg.Source = "uncheck.png";
            timeoffimg.Source = "check.png";
            selectServicePan.IsVisible = false;
            titlePan.IsVisible = true;
            appTimePan.IsVisible = false;
            offTimePan.IsVisible = true;

            bTimeOff = true;
        }
        private async void available_Clicked(object sender, EventArgs e)
        {
            availableimg.Source = "check.png";
            customimg.Source = "uncheck.png";
            timeStatus.Text = "Base on your availablity, hours, and booking preferences.";
            availableTimePan.IsVisible = true;
            customTimePan.IsVisible = false;
        }
        private async void custom_Clicked(object sender, EventArgs e)
        {
            availableimg.Source = "uncheck.png";
            customimg.Source = "check.png";
            timeStatus.Text = "Override your availablity, hours, and booking preferences.";
            availableTimePan.IsVisible = false;
            customTimePan.IsVisible = true;
        }
        private async void serviceItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;
            Service service = (Service)gridItem.BindingContext;
            Label lblCheck = (Label)gridItem.Children[0];
            Image lmgCheck = (Image)gridItem.Children[1];
            if (lblCheck.Text == "0")
            {
                lmgCheck.Source = "check.png";
                lblCheck.Text = "1";
                int index = m_lstServices.IndexOf(service);
                if (currentSetting.multi_service == 0 && currentIndex != -1)
                {
                    m_lstServices[currentIndex].image = "uncheck.png";
                    m_lstServices[currentIndex].check = "0";
                    m_lstServices[index].image = "check.png";
                    m_lstServices[index].check = "1";
                    Services.ItemsSource = new List<Service>();
                    Services.ItemsSource = m_lstServices;
                }
                currentIndex = index;
            }
            else if (currentSetting.multi_service == 1)
            {
                lmgCheck.Source = "uncheck.png";
                lblCheck.Text = "0";
            }
            if (currentSetting.multi_service == 1)
            {
                if (lstCurrentServices.Contains(service))
                    lstCurrentServices.Remove(service);
                else
                    lstCurrentServices.Add(service);
            }
            else
            {
                lstCurrentServices.Clear();
                lstCurrentServices.Add(service);
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (swAllday.IsToggled)
            {
                availableTimePan.IsVisible = true;
                tmCustom.IsVisible = false;
            }
            else
            {
                availableTimePan.IsVisible = false;
                tmCustom.IsVisible = true;
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {

        }
    }
}