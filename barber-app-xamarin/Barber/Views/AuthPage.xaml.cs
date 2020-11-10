using Barber.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {
        public AuthPage()
        {
            InitializeComponent();
            BindingContext = new AuthViewModel();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            GradientPan.HeightRequest = mainDisplayInfo.Height;

            MessagingCenter.Subscribe<AuthPage>(this, "goLoginTap", (sender) => {
                goLoginTap();
            });
        }
        private async void goLoginTap()
        {
            fName.Text = "";
            lName.Text = "";
            password.Text = "";
            email.Text = "";
            confirmpassword.Text = "";
            swbarber.IsToggled = false;
            showLoginTap();
        }
        private void loginTab(object sender, EventArgs e)
        {
            showLoginTap();
        }

        private void signupTab(object sender, EventArgs e)
        {
            loginForm.IsVisible = false;
            loginBorder.IsVisible = false;

            registerForm.IsVisible = true;
            registerBorder.IsVisible = true;
        }

        private void showLoginTap()
        {
            loginForm.IsVisible = true;
            loginBorder.IsVisible = true;

            registerForm.IsVisible = false;
            registerBorder.IsVisible = false;
        }
    }
}