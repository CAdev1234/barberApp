using System;
using System.Windows.Input;
using Xamarin.Forms;
using Barber.Models;

using Barber.Services;
using Barber.Views;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using Barber.Popups;

namespace Barber.ViewModels
{
    class AuthViewModel : BaseViewModel
    {
        public LoginModel _loginUser;
        public RegisterModel _registerUser;
        private PopupPage popupPage = new LoadPopupPage();

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public LoginModel LoginUser
        {
            get => _loginUser;
            set => SetProperty(ref _loginUser, value);
        }
        public RegisterModel RegisterUser
        {
            get => _registerUser;
            set => SetProperty(ref _registerUser, value);
        }

        public AuthViewModel()
        {
            LoginCommand = new Command(LoginExecute);
            RegisterCommand = new Command(RegisterExecute);
            LoginUser = new LoginModel();
            RegisterUser = new RegisterModel();
        }

        private void LoginExecute(object obj)
        {
            LoginRequest user = new LoginRequest();
            user.email = LoginUser.Email;
            user.password = LoginUser.Password;
            if(user.email == "" || user.email == null || !ValidationServices.EmailValidation(user.email))
            {
                HandleToast.Toast("Invalid Email.");
                return;
            }
            if (user.password == "" || user.password == null)
            {
                HandleToast.Toast("Invalid Password.");
                return;
            }

            LoginAsync(user);
        }
        
        private void RegisterExecute(object obj)
        {
            RegisterRequest user = new RegisterRequest();
            user.firstname = RegisterUser.FirstName;
            user.lastname = RegisterUser.LastName;
            user.email = RegisterUser.Email;
            user.password = RegisterUser.Password;
            user.password_confirmation = RegisterUser.ConfirmPassword;
            user.barber = RegisterUser.barber;
            user.rate = 0;
            user.device_token = SharedService.GetValue("device_token");
            user.iphone_device_token = "";

            if(user.device_token == "")
            {
                HandleToast.Toast("Network error.");
                return;
            }
            if (user.firstname == "" || user.firstname == null)
            {
                HandleToast.Toast("Invalid First Name.");
                return;
            }
            if (user.lastname == "" || user.lastname == null)
            {
                HandleToast.Toast("Invalid Last Name.");
                return;
            }
            if (user.email == "" || user.email == null || !ValidationServices.EmailValidation(user.email))
            {
                HandleToast.Toast("Invalid Email.");
                return;
            }
            if (user.password == "" || user.password == null)
            {
                HandleToast.Toast("Invalid Password.");
                return;
            }
            if (RegisterUser.Password != RegisterUser.ConfirmPassword)
            {
                HandleToast.Toast("Please input correct password.");
                return;
            }

            RegisterAsync(user);
        }
        private void GoMainPage()
        {
            if(!SharedService.GetUser().User.barber)
                Application.Current.MainPage = new MainPage();
            else
                Application.Current.MainPage = new BarberMainPage();
        }
        private async Task LoginAsync(LoginRequest user)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            try
            {
                var result = await ApiService.GetOneWithPost<LoginResponse, LoginRequest>(user, "login");
                if (result == null)
                {
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    return;
                }

                if (result.Success)
                {
                    HandleToast.Toast("Login success");
                    
                    SharedService.SetUser(result);
                    GoMainPage();
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    return;
                }
                else
                {
                    HandleToast.Toast("Login failed");
                }
            }
            catch (Exception e)
            {
                HandleToast.Toast("Something went wrong");
            }
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
        }
        private async Task RegisterAsync(RegisterRequest user)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            try
            {
                ResponseModel result = await ApiService.GetOneWithPost<ResponseModel, RegisterRequest>(user, "register");
                if(result == null)
                {
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    HandleToast.Toast("Something went wrong.");
                    return;
                }
                if (result.Success)
                {
                    HandleToast.Toast("Register success");
                    MessagingCenter.Send<AuthPage>(new AuthPage(), "goLoginTap");
                }
                else
                {
                    HandleToast.Toast("Register failed");
                }
            }
            catch(Exception e)
            {
                HandleToast.Toast("Something went wrong");
            }
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
        }

    }
}
