
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
    public partial class ReviewBarberPage : ContentPage
    {
        private int rate = -1;
        private User currentUser;
        public ReviewBarberPage(User user)
        {
            InitializeComponent();

            currentUser = user;

            rateInfo.ItemsSource = getRatingImage(0);
        }

        private async void ratingItemClicked(object sender, EventArgs e)
        {
            var stacklayout = (StackLayout)sender;
            Label lblCount = (Label)stacklayout.Children[0];
            rate = Int32.Parse(lblCount.Text);
            rateInfo.ItemsSource = getRatingImage(rate);
        }

        private List<RateImage> getRatingImage(int iCount)
        {
            List<RateImage> lstRates = new List<RateImage>();

            for (int i = 0; i < 5; i++)
            {
                RateImage newRate = new RateImage();
                newRate.count = i + 1;
                if (i < iCount)
                    newRate.image = "rating_star_on.png";
                else
                    newRate.image = "rating_star_off.png";
                lstRates.Add(newRate);
            }
            return lstRates;
        }

        private async void saveReview(object sender, EventArgs e)
        {
            ReviewBarber review = new ReviewBarber();
            review.content = Content.Text;
            review.rate = rate;
            review.barberid = currentUser.id;

            if (review.content == "" || review.content == null)
            {
                HandleToast.Toast("Invalid description.");
                return;
            }

            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, ReviewBarber>(review, "reviewbarber");
            if (res == null)
                return;

            if(res.Success)
            {
                HandleToast.Toast("Successfully reviewed.");
                MessagingCenter.Send<BarberViewPage>(new BarberViewPage(currentUser), "RefreshReview");
            }
        }
    }
}