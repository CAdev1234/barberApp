using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class BarberViewPage : ContentPage
    {
        List<Week> weeks = new List<Week>();
        public List<Service> lstServices = new List<Service>();
        User currentUser;
        private PopupPage popupPage = new LoadPopupPage();
        public BarberViewPage(User user, bool favBarber = false)
        {
            InitializeComponent();

            currentUser = user;

            if (favBarber || user.favBarber == 1)
            {
                lblAdd.TextColor = Color.FromHex("#db6e53");
                lblAdd.Text = "REMOVE";
                btnAdd.Source = "remove";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            clearTap();
            infoContent.IsVisible = true;
            lblInfo.TextColor = Color.FromHex("#ddc686");
            bxInfo.Color = Color.FromHex("#ddc686");

            Avatar.Source = currentUser.avatar;
            AvatarBack.Source = currentUser.avatar;
            BarberName.Text = currentUser.fullname;

            drawInfoPageAsync(currentUser);
            drawReviewPage(currentUser);
            drawServicePage(currentUser);

            MessagingCenter.Subscribe<BarberViewPage>(this, "RefreshReview", (sender) => {
                drawReviewPage(currentUser);
            });

            MessagingCenter.Subscribe<BarberViewPage>(this, "RefreshFavouriteBarber", (sender) => {
                lblAdd.TextColor = Color.FromHex("#db6e53");
                lblAdd.Text = "REMOVE";
                btnAdd.Source = "remove";
            });
        }
        private async void tapInfoClicked(Object sender, EventArgs e)
        {
            clearTap();
            var stackLayout = (StackLayout)sender;
            Label lblActive = (Label)stackLayout.Children[0];
            lblActive.TextColor = Color.FromHex("#ddc686");
            BoxView bxActive = (BoxView)stackLayout.Children[1];
            bxActive.Color = Color.FromHex("#ddc686");

            infoContent.IsVisible = true;
        }
        private async void tapReviewClicked(Object sender, EventArgs e)
        {
            clearTap();
            rateInfo.ItemsSource = getRatingImage(5, true);
            bxReview.Color = Color.FromHex("#ddc686");

            reviewContent.IsVisible = true;
        }
        private async void tapServiceClicked(Object sender, EventArgs e)
        {
            clearTap();
            var stackLayout = (StackLayout)sender;
            Label lblActive = (Label)stackLayout.Children[0];
            lblActive.TextColor = Color.FromHex("#ddc686");
            BoxView bxActive = (BoxView)stackLayout.Children[1];
            bxActive.Color = Color.FromHex("#ddc686");
            
            serviceContent.IsVisible = true;
        }
        private void clearTap()
        {
            infoContent.IsVisible = false;
            reviewContent.IsVisible = false;
            serviceContent.IsVisible = false;

            lblInfo.TextColor = Color.FromHex("#808080");
            bxInfo.Color = Color.FromHex("#808080");
            rateInfo.ItemsSource = getRatingImage(0, true);
            bxReview.Color = Color.FromHex("#808080");
            lblService.TextColor = Color.FromHex("#808080");
            bxService.Color = Color.FromHex("#808080");
        }
        private async Task drawInfoPageAsync(User user)
        {
            if (PopupNavigation.PopupStack.Count == 0) if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberInfoRequest res = await ApiService.GetOneWithPost<BarberInfoRequest, User>(user, "locationinfo");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                if (res.Info == null)
                {
                    infoPan.IsVisible = false;
                }
                else
                {
                    infoPan.IsVisible = true;
                    infophonepan.IsVisible = true;

                    infobio.Text = res.Info.bio;
                    infophone.Text = res.Info.phone;
                    if (res.Info.bio == null)
                        infobio.IsVisible = false;
                    if (res.Info.phone == null)
                        infophonepan.IsVisible = false;
                }

                if (res.Location != null)
                {
                    grid_location_hours.IsVisible = true;

                    ShopName.Text = res.Location.shopName;
                    if(res.Location.locationType == "Mobile (House-Calls Only)")
                    {
                        LocationType.Text = "Mobile";
                        frLocationType.BackgroundColor = Color.FromHex("#5aadff");
                    }
                    else if(res.Location.locationType == "Commercial (Shop, Studio, etc.)")
                    {
                        LocationType.Text = "Commercial";
                        frLocationType.BackgroundColor = Color.FromHex("#5aff85");
                    }
                    else
                    {
                        LocationType.Text = "Resudential";
                        frLocationType.BackgroundColor = Color.FromHex("#ddc686");
                    }
                    StreetAddress.Text = res.Location.streetAddress + ", " + res.Location.zipcode;

                    currentUser.location = res.Location;
                    
                    addWeekData("Sunday", res.Location.sunday);
                    addWeekData("Monday", res.Location.monday);
                    addWeekData("Tuesday", res.Location.tuesday);
                    addWeekData("Wednesday", res.Location.wednesday);
                    addWeekData("Thursday", res.Location.thursday);
                    addWeekData("Friday", res.Location.friday);
                    addWeekData("Saturday", res.Location.saturday);
                    Weeks.ItemsSource = weeks;

                    Position pos = new Position(currentUser.location.latitude, currentUser.location.longitude);
                    MapSpan mapSpan = MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(10));

                    myMap.MoveToRegion(mapSpan);
                    Pin pin = new Pin
                    {
                        Label = "",
                        Address = "",
                        Type = PinType.Place,
                        Position = pos
                    };
                    myMap.Pins.Clear();
                    myMap.Pins.Add(pin);
                }
                else
                {
                    grid_location_hours.IsVisible = false;
                }

                if (res.Gallerys.Count > 0)
                {
                    Gallerys.IsVisible = true;
                    foreach (Gallery item in res.Gallerys)
                    {
                        item.image = ApiService.SERVER_HOST + item.image;
                    }
                    Gallerys.ItemsSource = res.Gallerys;
                }
                else
                {
                    Gallerys.IsVisible = false;
                }
            }
        }
        private async void drawReviewPage(User user)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            ReviewRequest res = await ApiService.GetOneWithPost<ReviewRequest, User>(user, "review");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                List<Review> lstTmp = new List<Review>();
                foreach (Review item in res.Reviews)
                {
                    item.Client.avatar = ApiService.SERVER_HOST + item.Client.avatar;
                    item.Client.fullname = item.Client.firstname + " " + item.Client.lastname;
                    item.RateImages = getRatingImage(item.rate);
                    string[] times = item.created_at.Split('T');
                    item.created_at = times[0];
                    lstTmp.Add(item);
                }
                Reviews.ItemsSource = lstTmp;
            }
        }
        private async Task drawServicePage(User user)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            ServiceRequest res = await ApiService.GetOneWithPost<ServiceRequest, User>(user, "service");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                List<Service> lstTmp = new List<Service>();
                foreach (Service item in res.Services)
                {
                    item.price = item.price;
                    item.strprice = "$"+item.price;
                    item.check = "0";
                    item.duration = getDuration(item.hour, item.min);
                    item.image = "uncheck.png";
                    
                    lstTmp.Add(item);
                }
                Services.ItemsSource = lstTmp;
                lstServices = lstTmp;
            }
        }

        private string getDuration(string hour, string min)
        {
            string duration = "";
            if (hour != "0" && hour != null)
                duration = hour + " hour ";
            if (min != "0" && min != null)
                duration += min + " minutes";
            return duration;
        }

        private async void serviceItemClicked(object sender, EventArgs e)
        {
            if(lstServices.Count == 0)
            {
                HandleToast.Toast("This barber does not have any service.");
                return;
            }
            var gridItem = (Grid)sender;
            Service service = (Service)gridItem.BindingContext;

            foreach(Service item in lstServices)
            {
                item.image = "uncheck.png";
                item.check = "0";
            }
            await Navigation.PushAsync(new BookingPage(currentUser, service, lstServices));
        }

        private void addWeekData(string day, string hour)
        {
            if (hour != null)
            {
                Week oneweek = new Week();
                oneweek.day = day;
                oneweek.hour = hour;
                weeks.Add(oneweek);
            }
        }

        private async void favouriteBarber(object sender, EventArgs e)
        {
            FavouriteRequest res = await ApiService.GetOneWithPost<FavouriteRequest, User>(currentUser, "favourite");
            if (res == null)
                return;

            if (res.Success)
            {
                if (res.Result == 1)
                {
                    HandleToast.Toast("Added Successfully!");
                    lblAdd.TextColor = Color.FromHex("#db6e53");
                    lblAdd.Text = "REMOVE";
                    btnAdd.Source = "remove";
                }
                else
                {
                    HandleToast.Toast("Removed Successfully!");
                    lblAdd.TextColor = Color.FromHex("#ddc686");
                    lblAdd.Text = "ADD";
                    btnAdd.Source = "add";
                }
                MessagingCenter.Send<HomePage>(new HomePage(), "RefreshFavouriteBarber");
            }
        }
        private List<RateImage> getRatingImage(int iCount, bool isView = false)
        {
            List<RateImage> lstRates = new List<RateImage>();

            for (int i = 0; i < 5; i++)
            {
                RateImage newRate = new RateImage();
                newRate.count = i + 1;
                if (i < iCount)
                    newRate.image = "rating_star_on.png";
                else
                {
                    newRate.image = "rating_star_off.png";
                    if (isView)
                        newRate.image = "rating_star_view.png";
                }
                lstRates.Add(newRate);
            }
            return lstRates;
        }
        private async void reviewBarber(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ReviewBarberPage(currentUser));
        }
        private async void bookAppointment(object sender, EventArgs e)
        {
            if(lstServices.Count == 0)
            {
                HandleToast.Toast("This barber does not have any services.");
                return;
            }
            Navigation.PushAsync(new BookingPage(currentUser, null, lstServices));
        }
        private async void shareBarber(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "You should definitely check out this app, theCut. See if your barber is on it, or find other barbers in your area. You can schedule your next haircut in advance, pay and tip all through the app. No more waiting in the shop. It's a game changer. https://book.thecut.co/client-recommend",
                Title = "Share With"
            });
        }
        private async void myMap_Clicked(object sender, EventArgs e)
        {
            var request = string.Format("http://maps.google.com/?q=" + currentUser.location.latitude + "," + currentUser.location.longitude + "");
            Device.OpenUri(new Uri(request));
        }
    }
}