using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Barber.Services;
using Barber.Views;
using Barber.Models;
using Xamarin.Essentials;


namespace Barber
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                HandleToast.Toast("Network error.");
                return;
            }

            DependencyService.Register<MockDataStore>();
            try
            {
                LoginResponse res = SharedService.GetUser();
                if (res.Success)
                {
                    if (res.User.barber)
                        App.Current.MainPage = new BarberMainPage();
                    else
                        App.Current.MainPage = new MainPage();
                    return;
                }
            }
            catch (Exception e)
            {
            }
            MainPage = new AuthPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
