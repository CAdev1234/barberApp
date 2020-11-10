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
    public partial class ProfileLinkPage : ContentPage
    {
        public ProfileLinkPage()
        {
            InitializeComponent();
        }

        private void saveSetting(object sender, EventArgs e)
        {

        }
    }
}