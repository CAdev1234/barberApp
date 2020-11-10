
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
    public partial class EditInfoPage : ContentPage
    {
        public EditInfoPage(BarberInfo info)
        {
            InitializeComponent();
            edtBio.Text = info.bio;
            edtPhone.Text = info.phone;
        }

        private async void save(object sender, EventArgs e)
        {
            string bio = edtBio.Text;
            string phone = edtPhone.Text;
            BarberInfo info = new BarberInfo();
            info.bio = bio;
            info.phone = phone;
            info.barberid = SharedService.GetUser().User.id;

            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, BarberInfo>(info, "editinfo");
            if (res == null)
                return;

            if (res.Success)
            {
                MessagingCenter.Send<BarberHomePage>(new BarberHomePage(), "RefreshLocation");
                Navigation.PopAsync(true);
            }
        }
    }
}