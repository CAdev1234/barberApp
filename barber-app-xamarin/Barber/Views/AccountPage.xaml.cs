using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Plugin.Media;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public string ImageBase64 { get; set; }
        public string imageName = "";
        public int type = 0;
        private PopupPage popupPage = new LoadPopupPage();

        public AccountPage(string name)
        {
            InitializeComponent();

            if (name == "Edit Account")
            {
                TitleBar.Text = "ACCOUNT SETTING";
                Avatar.Source = SharedService.GetUser().User.avatar;
                FName.Text = SharedService.GetUser().User.firstname;
                LName.Text = SharedService.GetUser().User.lastname;
                Email.Text = SharedService.GetUser().User.email;
                passwordPan.IsVisible = false;
                accountPan.IsVisible = true;
            }
            else
            {
                TitleBar.Text = "CHANGE PASSWORD";
                type = 1;
                accountPan.IsVisible = false;
                passwordPan.IsVisible = true;
            }
        }

        private async void OpenGallery(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null) return;
                var stream = file.GetStream();
                byte[] filebytearray = new byte[stream.Length];
                var _imageBase64 = Convert.ToBase64String(File.ReadAllBytes(file.Path));
                ImageBase64 = _imageBase64;
                int index = (file.Path.LastIndexOf("/", StringComparison.Ordinal) + 1);
                Avatar.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(ImageBase64)));
                imageName = file.Path;
            }
            catch (Exception ex)
            {
                string erro = ex.Message;
            }
        }

        private async void saveSetting(object sender, EventArgs e)
        {
            if (type == 0)
            {
                string fname = FName.Text;
                string lname = LName.Text;
                string email = Email.Text;
                if (fname == "" || fname == null)
                {
                    HandleToast.Toast("First Name is required!");
                    return;
                }
                if (lname == "" || lname == null)
                {
                    HandleToast.Toast("Last Name is required!");
                    return;
                }
                if (email == "" || lname == null)
                {
                    HandleToast.Toast("Email is required!");
                    return;
                }

                User user = SharedService.GetUser().User;
                user.email = email;
                user.firstname = fname;
                user.lastname = lname;

                if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);

                if (imageName != "")
                {
                    ImageUploadRequest res1 = await ApiService.uploadPhotoWithPost<ImageUploadRequest>("uploadprofile", imageName);
                    if (res1 == null)
                        return;
                    SharedService.SetValue("avatar", ApiService.SERVER_HOST + res1.ImageName);
                }

                SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, User>(user, "changeaccount");

                if (res == null)
                {
                    if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                    return;
                }

                HandleToast.Toast("Successfully changed.");

                SharedService.SetValue("firstname", fname);
                SharedService.SetValue("lastname", lname);
                SharedService.SetValue("fullname", fname + " " + lname);
                SharedService.SetValue("email", email);

                MessagingCenter.Send<HomePage>(new HomePage(), "RefreshProfile");
                MessagingCenter.Send<BarberHomePage>(new BarberHomePage(), "RefreshProfile");
            }
            else
            {
                string currentPwd = CurrentPwd.Text;
                string newpwd = Newpwd.Text;
                string confirmpwd = Confirmpwd.Text;
                if (currentPwd == null || currentPwd == "")
                {
                    HandleToast.Toast("Current password is required.");
                    return;
                }
                if (newpwd == null || newpwd == "")
                {
                    HandleToast.Toast("New password is required.");
                    return;
                }
                if (newpwd != confirmpwd)
                {
                    HandleToast.Toast("Passwords do not match.");
                    return;
                }
                if (newpwd.Length < 8 || newpwd.Length > 20)
                {
                    HandleToast.Toast("New password must be 8 - 20 characters.");
                    return;
                }

                ChangePwd pwd = new ChangePwd();
                pwd.currentPwd = currentPwd;
                pwd.newPwd = newpwd;

                if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
                SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, ChangePwd>(pwd, "changepassword");
                if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                if (res == null)
                    return;

                if (!res.Success)
                {
                    HandleToast.Toast("Current password is incorrect.");
                    return;
                }
                HandleToast.Toast("Password successfully changed.");
                CurrentPwd.Text = "";
                Confirmpwd.Text = "";
                Newpwd.Text = "";
            }
        }
    }
}