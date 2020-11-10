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
    public partial class BarberFilterPage : ContentPage
    {
        public BarberFilterPage()
        {
            InitializeComponent();

            Filter filter = SharedService.getFilter();
            if(filter.request == "0")
            {
                reqimg.Source = "uncheck.png";
                reqcheck.Text = "0";
            }
            if(filter.cancel == "0")
            {
                canimg.Source = "uncheck.png";
                cancheck.Text = "0";
            }
            if (filter.confirm == "0")
            {
                conimg.Source = "uncheck.png";
                concheck.Text = "0";
            }
            if (filter.complete == "0")
            {
                comimg.Source = "uncheck.png";
                comcheck.Text = "0";
            }
            if (filter.noshow == "0")
            {
                noimg.Source = "uncheck.png";
                nocheck.Text = "0";
            }
        }

        private async void filter(object sender, EventArgs e)
        {
            var grid = (Grid)sender;
            Image btnFilter = (Image)grid.Children[2];
            Label lblCheck = (Label)grid.Children[1];
            Label lblFilter = (Label)grid.Children[3];
            Label lblNumber = (Label)grid.Children[0];
            if (lblCheck.Text == "1")
            {
                btnFilter.Source = "uncheck.png";
                lblCheck.Text = "0";
                SharedService.SetValue(lblNumber.Text, "0");
            }
            else
            {
                btnFilter.Source = "check.png";
                lblCheck.Text = "1";
                SharedService.SetValue(lblNumber.Text, "1");
            }
        }

        private async void saveFilter(object sender, EventArgs e)
        {
            MessagingCenter.Send<BarberBookPage>(new BarberBookPage(), "RefreshBook");
        }
    }
}