using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Barber.Models;
using Barber.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Barber.ViewModels
{
    public class PaymentPageViewModel: INotifyPropertyChanged
    {
        public ICommand PayCommand { get; set; }
        public ICommand OnPaymentOptionSelected { get; set; }
        public PaymentOptionEnum PaymentOptionEnum { get; set; }

        public CardInfo CardInfo { get; set; } = new CardInfo();
        IPayService _payService;

        string paymentClientToken = "eyJ2ZXJzaW9uIjoyLCJhdXRob3JpemF0aW9uRmluZ2VycHJpbnQiOiJleUowZVhBaU9pSktWMVFpTENKaGJHY2lPaUpGVXpJMU5pSXNJbXRwWkNJNklqSXdNVGd3TkRJMk1UWXRjMkZ1WkdKdmVDSXNJbWx6Y3lJNkltaDBkSEJ6T2k4dllYQnBMbk5oYm1SaWIzZ3VZbkpoYVc1MGNtVmxaMkYwWlhkaGVTNWpiMjBpZlEuZXlKbGVIQWlPakUyTURBek9ERTRNRE1zSW1wMGFTSTZJak5oT1dKaVltUXlMV05sWldFdE5EZzROaTA0WkdZNUxXVXpOakpqTWpVd09EbGhZU0lzSW5OMVlpSTZJbmR3TkRNNVl6SjVhMnBtYW5CNGVEWWlMQ0pwYzNNaU9pSm9kSFJ3Y3pvdkwyRndhUzV6WVc1a1ltOTRMbUp5WVdsdWRISmxaV2RoZEdWM1lYa3VZMjl0SWl3aWJXVnlZMmhoYm5RaU9uc2ljSFZpYkdsalgybGtJam9pZDNBME16bGpNbmxyYW1acWNIaDROaUlzSW5abGNtbG1lVjlqWVhKa1gySjVYMlJsWm1GMWJIUWlPbVpoYkhObGZTd2ljbWxuYUhSeklqcGJJbTFoYm1GblpWOTJZWFZzZENKZExDSnpZMjl3WlNJNld5SkNjbUZwYm5SeVpXVTZWbUYxYkhRaVhTd2liM0IwYVc5dWN5STZlMzE5LkJBQnNPSFQ5R1ozMGxuS1dWcHR3VEFVdUkxTDNUX3pZS1lZeWt2OE1NV3MyYTBGMWJxTUJKYmFHaE5aTndSZDIyUlJJT1kxTlV6Tjd1RmhPOXQwRlVBIiwiY29uZmlnVXJsIjoiaHR0cHM6Ly9hcGkuc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbTo0NDMvbWVyY2hhbnRzL3dwNDM5YzJ5a2pmanB4eDYvY2xpZW50X2FwaS92MS9jb25maWd1cmF0aW9uIiwiZ3JhcGhRTCI6eyJ1cmwiOiJodHRwczovL3BheW1lbnRzLnNhbmRib3guYnJhaW50cmVlLWFwaS5jb20vZ3JhcGhxbCIsImRhdGUiOiIyMDE4LTA1LTA4IiwiZmVhdHVyZXMiOlsidG9rZW5pemVfY3JlZGl0X2NhcmRzIl19LCJjbGllbnRBcGlVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvd3A0MzljMnlramZqcHh4Ni9jbGllbnRfYXBpIiwiZW52aXJvbm1lbnQiOiJzYW5kYm94IiwibWVyY2hhbnRJZCI6IndwNDM5YzJ5a2pmanB4eDYiLCJhc3NldHNVcmwiOiJodHRwczovL2Fzc2V0cy5icmFpbnRyZWVnYXRld2F5LmNvbSIsImF1dGhVcmwiOiJodHRwczovL2F1dGgudmVubW8uc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbSIsInZlbm1vIjoib2ZmIiwiY2hhbGxlbmdlcyI6W10sInRocmVlRFNlY3VyZUVuYWJsZWQiOnRydWUsImFuYWx5dGljcyI6eyJ1cmwiOiJodHRwczovL29yaWdpbi1hbmFseXRpY3Mtc2FuZC5zYW5kYm94LmJyYWludHJlZS1hcGkuY29tL3dwNDM5YzJ5a2pmanB4eDYifSwicGF5cGFsRW5hYmxlZCI6dHJ1ZSwicGF5cGFsIjp7ImJpbGxpbmdBZ3JlZW1lbnRzRW5hYmxlZCI6dHJ1ZSwiZW52aXJvbm1lbnROb05ldHdvcmsiOmZhbHNlLCJ1bnZldHRlZE1lcmNoYW50IjpmYWxzZSwiYWxsb3dIdHRwIjp0cnVlLCJkaXNwbGF5TmFtZSI6Ik1WUEN1dCIsImNsaWVudElkIjoiQVk5bkxkRHNqclYxWHVReDRPRjh2aGZqLWxzMlF1TlBXSjFEdmFIQTk0S3R0LU1uLWUtY1MzYUhhSlRUODROUnpiWkxxMC1tV2V3aGY4Mm8iLCJwcml2YWN5VXJsIjoiaHR0cDovL2V4YW1wbGUuY29tL3BwIiwidXNlckFncmVlbWVudFVybCI6Imh0dHA6Ly9leGFtcGxlLmNvbS90b3MiLCJiYXNlVXJsIjoiaHR0cHM6Ly9hc3NldHMuYnJhaW50cmVlZ2F0ZXdheS5jb20iLCJhc3NldHNVcmwiOiJodHRwczovL2NoZWNrb3V0LnBheXBhbC5jb20iLCJkaXJlY3RCYXNlVXJsIjpudWxsLCJlbnZpcm9ubWVudCI6Im9mZmxpbmUiLCJicmFpbnRyZWVDbGllbnRJZCI6Im1hc3RlcmNsaWVudDMiLCJtZXJjaGFudEFjY291bnRJZCI6Im12cGN1dCIsImN1cnJlbmN5SXNvQ29kZSI6IlVTRCJ9fQ==";
        const string MerchantId = "wp439c2ykjfjpxx6";
        const double AmountToPay = 222;
        
        public PaymentPageViewModel()
        {
            _payService= DependencyService.Get<IPayService>();
            PayCommand = new Command(async () => await CreatePayment());
            OnPaymentOptionSelected = new Command<PaymentOptionEnum>((data) => {
                PaymentOptionEnum = data;

                if (PaymentOptionEnum != PaymentOptionEnum.CreditCard)
                    PayCommand.Execute(null);
             });
            GetPaymentConfig();
        }

        async Task GetPaymentConfig()
        {
            await _payService.InitializeAsync(paymentClientToken);
        }

        async Task CreatePayment()
        {
            if (_payService.CanPay)
            {
                try
                {
                    _payService.OnTokenizationSuccessful += OnTokenizationSuccessful;
                    _payService.OnTokenizationError += OnTokenizationError;

                        switch (PaymentOptionEnum)
                        {
                            case PaymentOptionEnum.PayPal:
                                await _payService.TokenizePayPal();
                                break;
                            default:
                                break;
                        }
            }
            catch (Exception ex)
                {
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    await App.Current.MainPage.DisplayAlert("Error", "Unable to process payment", "Ok");
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    await App.Current.MainPage.DisplayAlert("Error", "Payment not available", "Ok");
                });
            }
        }

        async void OnTokenizationSuccessful(object sender, string e)
        {
            _payService.OnTokenizationSuccessful -= OnTokenizationSuccessful;
            System.Diagnostics.Debug.WriteLine($"Payment Authorized - {e}");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            await App.Current.MainPage.DisplayAlert("Success", $"Payment Authorized: the token is{e}", "Ok");
           
        }

        async void OnTokenizationError(object sender, string e)
        {
            _payService.OnTokenizationError -= OnTokenizationError;
            System.Diagnostics.Debug.WriteLine(e);
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            await App.Current.MainPage.DisplayAlert("Error", "Unable to process payment", "Ok");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
