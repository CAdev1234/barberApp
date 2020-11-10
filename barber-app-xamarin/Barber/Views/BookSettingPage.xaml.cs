
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookSettingPage : ContentPage
    {
        public BookSetting currentSetting = new BookSetting();
        private PopupPage popupPage = new LoadPopupPage();
        public BookSettingPage()
        {
            InitializeComponent();

            currentSetting.barberid = SharedService.GetUser().User.id;
            currentSetting.auto_confirm = 0;
            currentSetting.auto_comment = null;
            currentSetting.multi_service = 0;
            currentSetting.require_phone = 0;
            currentSetting.last_limit_hour = "0 hour";
            currentSetting.last_limit_min = "0 minute";
            currentSetting.future_limit = "1 year";
            currentSetting.requrring_limit = "Disable";

            drawPage();
        }
        private async void drawPage()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookSettingRequest res = await ApiService.GetOneWithGet<BookSettingRequest>("getbooksetting");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                currentSetting = res.BookSetting;
                if(currentSetting.auto_confirm == 1)
                {
                    swAutoConfirm.IsToggled = true;
                    swAutoConfirm.ThumbColor = Color.FromHex("#ddc686");
                    lblAutoConfirm.TextColor = Color.FromHex("#ffffff");
                    AutoComment.Text = "Recurring appointments must still be manually confirmed.";
                }
                else
                    AutoComment.Text = "All appointments must be manually confirmed.";

                if (currentSetting.multi_service == 1)
                {
                    swMultiService.IsToggled = true;
                    swMultiService.ThumbColor = Color.FromHex("#ddc686");
                    lblMultiService.TextColor = Color.FromHex("#ffffff");
                    MultiComment.Text = "Appointments can be booked with multiple services.";
                }
                else
                    MultiComment.Text = "Appointments can be booked with only one services.";

                if (currentSetting.require_phone == 1)
                {
                    swRequirePhone.IsToggled = true;
                    swRequirePhone.ThumbColor = Color.FromHex("#ddc686");
                    lblRequirePhone.TextColor = Color.FromHex("#ffffff");
                    PhoneComment.Text = "Clients must provide a phone number when booking.";
                }
                else
                    PhoneComment.Text = "Clients will not have to provide a phone number when booking.";

                if (currentSetting.auto_comment != null)
                {
                    autoComment.Text = currentSetting.auto_comment;
                }
            }
        }

        private async void save(object sender, EventArgs e)
        {
            currentSetting.auto_comment = autoComment.Text;
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, BookSetting>(currentSetting, "booksetting"); ;
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if(res.Success)
            {
                HandleToast.Toast("Successfully saved");
            }
        }

        private void swAutoConfirm_Toggled(object sender, ToggledEventArgs e)
        {
            if(swAutoConfirm.IsToggled)
            {
                currentSetting.auto_confirm = 1;
                swAutoConfirm.ThumbColor = Color.FromHex("#ddc686");
                lblAutoConfirm.TextColor = Color.FromHex("#ffffff");
                AutoComment.Text = "Recurring appointments must still be manually confirmed.";
            }
            else
            {
                currentSetting.auto_confirm = 0;
                swAutoConfirm.ThumbColor = Color.FromHex("#808080");
                lblAutoConfirm.TextColor = Color.FromHex("#808080");
                AutoComment.Text = "All appointments must be manually confirmed.";
            }
        }

        private void swMultiService_Toggled(object sender, ToggledEventArgs e)
        {
            if (swMultiService.IsToggled)
            {
                currentSetting.multi_service = 1;
                swMultiService.ThumbColor = Color.FromHex("#ddc686");
                lblMultiService.TextColor = Color.FromHex("#ffffff");
                MultiComment.Text = "Appointments can be booked with multiple services.";
            }
            else
            {
                currentSetting.multi_service = 0;
                swMultiService.ThumbColor = Color.FromHex("#808080");
                lblMultiService.TextColor = Color.FromHex("#808080");
                MultiComment.Text = "Appointments can be booked with only one services.";
            }
        }

        private void swRequirePhone_Toggled(object sender, ToggledEventArgs e)
        {
            if (swRequirePhone.IsToggled)
            {
                currentSetting.require_phone = 1;
                swRequirePhone.ThumbColor = Color.FromHex("#ddc686");
                lblRequirePhone.TextColor = Color.FromHex("#ffffff");
                PhoneComment.Text = "Clients must provide a phone number when booking.";
            }
            else
            {
                currentSetting.require_phone = 0;
                swRequirePhone.ThumbColor = Color.FromHex("#808080");
                lblRequirePhone.TextColor = Color.FromHex("#808080");
                PhoneComment.Text = "Clients will not have to provide a phone number when booking.";
            }
        }

        private void pkInterval_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pkLastHour_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pkLastMin_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pkFutureYear_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pkRecurring_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}