using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Plugin.Media;
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
    public partial class BarberHomePage : ContentPage
    {
        List<Week> weeks = new List<Week>();
        private LocationInfo currentLocation = new LocationInfo();
        private BarberInfo currentInfo = new BarberInfo();
        private List<Gallery> currentGallerys = new List<Gallery>();
        private PopupPage popupPage = new LoadPopupPage();
        private int m_newAlarm = 0;
        public BarberHomePage()
        {
            InitializeComponent();

            drawNotification();
            drawProfile();
            drawInfoPageAsync();
            drawServicePage();
            drawReviewPage();

            MessagingCenter.Subscribe<BarberHomePage>(this, "RefreshProfile", (sender) =>
            {
                drawProfile();
            });
            MessagingCenter.Subscribe<BarberHomePage>(this, "RefreshLocation", (sender) =>
            {
                drawInfoPageAsync();
            });
            MessagingCenter.Subscribe<BarberHomePage>(this, "RefreshService", (sender) =>
            {
                drawServicePage();
            });
            MessagingCenter.Subscribe<BarberHomePage>(this, "RefreshReview", (sender) =>
            {
                drawReviewPage();
            });
            MessagingCenter.Subscribe<Object>(this, "NewNotification", (sender) => {
                drawNewAlarm();
            });

            if (SharedService.getFilter().request == "")
            {
                Filter filter = new Filter();
                filter.request = "1";
                filter.cancel = "1";
                filter.confirm = "1";
                filter.complete = "1";
                filter.noshow = "1";
                SharedService.SetFilter(filter);
            }

            clearTap();
            infoContent.IsVisible = true;
            lblInfo.TextColor = Color.FromHex("#ddc686");
            bxInfo.Color = Color.FromHex("#ddc686");

            currentInfo.bio = "";
            currentInfo.phone = "";
        }
        private async void drawNewAlarm()
        {
            m_newAlarm++;
            await Task.Run(() => {
                Device.InvokeOnMainThreadAsync(() => {
                    NewBadge.BadgeText = m_newAlarm.ToString();
                });
            });
        }
        private async void drawNotification()
        {
            NewAlarmRequest res = await ApiService.GetOneWithGet<NewAlarmRequest>("newalarm");
            if (res == null)
                return;

            if (res.Success)
            {
                NewBadge.BadgeText = m_newAlarm.ToString();
            }
        }
        private void drawProfile()
        {
            string fullname = SharedService.GetUser().User.fullname;
            if (fullname != "")
                FullName.Text = fullname;
            string avatar = SharedService.GetUser().User.avatar;
            if (avatar != "")
                Avatar.Source = avatar;
        }
        private async Task drawInfoPageAsync()
        {
            User user = SharedService.GetUser().User;
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberInfoRequest res = await ApiService.GetOneWithPost<BarberInfoRequest, User>(user, "locationinfo");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                actionPan.IsVisible = true;
                if(res.Location == null)
                {
                    LocationPan.IsVisible = false;
                    currentLocation = new LocationInfo();

                    var request = new GeolocationRequest(GeolocationAccuracy.Best);
                    var position = await Geolocation.GetLocationAsync(request);
                    setMap(new Position(position.Latitude, position.Longitude));
                }
                else
                {
                    LocationPan.IsVisible = true;

                    currentLocation = res.Location;
                
                    ShopName.Text = res.Location.shopName;

                    ShopName.Text = res.Location.shopName;
                    if (res.Location.locationType == "Mobile (House-Calls Only)")
                    {
                        LocationType.Text = "Mobile";
                        frLocationType.BackgroundColor = Color.FromHex("#5aadff");
                    }
                    else if (res.Location.locationType == "Commercial (Shop, Studio, etc.)")
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

                    weeks.Clear();

                    addWeekData("Sunday", res.Location.sunday);
                    addWeekData("Monday", res.Location.monday);
                    addWeekData("Tuesday", res.Location.tuesday);
                    addWeekData("Wednesday", res.Location.wednesday);
                    addWeekData("Thursday", res.Location.thursday);
                    addWeekData("Friday", res.Location.friday);
                    addWeekData("Saturday", res.Location.saturday);

                    Weeks.ItemsSource = new List<Week>();
                    Weeks.ItemsSource = weeks;

                    setMap(new Position(res.Location.latitude, res.Location.longitude));
                }

                if (res.Info == null)
                {
                    infoPan.IsVisible = false;
                    currentInfo = new BarberInfo();
                }
                else
                {
                    infoPan.IsVisible = true;
                    infophonepan.IsVisible = true;

                    currentInfo = res.Info;
                    infobio.Text = res.Info.bio;
                    infophone.Text = res.Info.phone;
                    if (res.Info.bio == null)
                        infobio.IsVisible = false;
                    if (res.Info.phone == null)
                        infophonepan.IsVisible = false;
                }

                if (res.Gallerys.Count == 0)
                {
                    galleryPan.IsVisible = false;
                    currentGallerys = new List<Gallery>();
                }
                else
                {
                    galleryPan.IsVisible = true;
                    foreach (Gallery item in res.Gallerys)
                        item.image = ApiService.SERVER_HOST + item.image;

                    Gallerys.HeightRequest = res.Gallerys.Count * 155;
                    Gallerys.ItemsSource = res.Gallerys;
                    currentGallerys = res.Gallerys;
                }
            }
        }
        private async void drawReviewPage()
        {
            User user = SharedService.GetUser().User;
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

                    DateTime formatDate = DateTime.Parse(item.created_at);
                    item.created_at = formatDate.ToString("MM/dd/yyyy");

                    lstTmp.Add(item);
                }

                if(lstTmp.Count > 0)
                {
                    NoReview.IsVisible = false;
                    HasReview.IsVisible = true;
                }

                Reviews.ItemsSource = lstTmp;
            }
        }
        private async Task drawServicePage()
        {
            User user = SharedService.GetUser().User;
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
                    item.strprice = "$" + item.price;
                    item.check = "0";
                    item.image = "uncheck.png";
                    item.duration = getDuration(item.hour, item.min);
                    lstTmp.Add(item);
                }

                if(lstTmp.Count > 0)
                {
                    NoService.IsVisible = false;
                    Services.IsVisible = true;
                }

                Services.ItemsSource = lstTmp;
            }
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

        private string getDuration (string hour, string min)
        {
            string duration = "";
            if (hour != "0" && hour != null)
                duration = hour + " hour ";
            if (min != "0" && min != null)
                duration += min + " minutes";
            return duration;
        }
        private async void editProfile(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AccountPage("Edit Account"));
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
                    if(isView)
                        newRate.image = "rating_star_view.png";
                }
                lstRates.Add(newRate);
            }
            return lstRates;
        }

        private async void editLocation(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditLocationPage(currentLocation));
        }
        private async void createService(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateServicePage(null));
        }
        private async void editService(object sender, EventArgs e)
        {
            var grid = (StackLayout)sender;
            Service service = (Service)grid.BindingContext;
            await Navigation.PushAsync(new CreateServicePage(service));
        }
        private async void editInfo(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditInfoPage(currentInfo));
        }
        private async void editPhoto(object sender, EventArgs e)
        {

        }
        private async void mobilePay(object sender, EventArgs e)
        {

        }
        private async void proLink(object sender, EventArgs e)
        {

        }
        private async void proPhoto(object sender, EventArgs e)
        {

        }
        private async void MyImage_Clicked(object sender, EventArgs e)
        {
            var gallery = ((ImageButton)sender).BindingContext;
            Gallery selectedGallery = (Gallery)gallery;
            int position = currentGallerys.IndexOf(selectedGallery);
            await Navigation.PushAsync(new GalleryViewPage(currentGallerys, position));
        }

        private async void openGallery(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();
                if (file == null)
                    return;

                if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
                GalleryRequest res = await ApiService.uploadPhotoWithPost<GalleryRequest>("addgallery", file.Path);
                if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
                if (res.Success)
                {
                    foreach (Gallery item in res.Gallerys)
                        item.image = ApiService.SERVER_HOST + item.image;

                    galleryPan.IsVisible = true;

                    currentGallerys = res.Gallerys;
                    Gallerys.ItemsSource = currentGallerys;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void myMap_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditLocationPage(currentLocation));
        }
        private void setMap(Position pos)
        {
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

        private async void showNotificationPage(object sender, EventArgs e)
        {
            NewBadge.BadgeText = "0";
            m_newAlarm = 0;
            await Navigation.PushAsync(new NotificationPage());
        }
    }
}