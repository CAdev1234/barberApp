
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Plugin.Geolocator;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Barber.ViewModels
{
    class SearchViewModel : BaseViewModel
    {
        private PopupPage popupPage = new LoadPopupPage();
        private List<User> _barbers;
        public List<User> AllBarbers
        {
            get => _barbers;
            set => SetProperty(ref _barbers, value);
        }
        public SearchViewModel(SearchData newData)
        {
            drawBarbersAsync(newData);
        }

        public async Task drawBarbersAsync(SearchData newData)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SearchRequest res = await ApiService.GetOneWithPost<SearchRequest, SearchData>(newData, "searchbarber");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                Location sourceCoordinates = new Location(0, 0);
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best);
                    var position = await Geolocation.GetLocationAsync(request);
                    sourceCoordinates = new Location(position.Latitude, position.Longitude);
                }
                catch (Exception error)
                {
                    string message = error.Message;
                    HandleToast.Toast("Cannot find your position. Please sure your network connection.");
                    return;
                }

                LocationInfo newlocation = new LocationInfo();
                List<User> lstUsers = new List<User>();

                newlocation.wholeAddress = "";
                foreach(User user in res.searchBarbers)
                {
                    user.avatar = ApiService.SERVER_HOST + user.avatar;
                    user.fullname = user.firstname + " " + user.lastname;
                    user.iReview = user.reviews.Count;
                    if (user.location == null)
                        continue;
                    else
                        user.location.wholeAddress = user.location.city + ", " + user.location.streetAddress + ", " + user.location.zipcode;

                    user.rateImages = getRatingImage(user.star);
                    user.StrReview = "( " + user.iReview + " )";
                    user.StrRate = user.rate.ToString();
                    if (user.iReview == 0)
                        user.StrReview = "";

                    Location destinationCoordinates = new Location(user.location.latitude, user.location.longitude);
                    double distance = Location.CalculateDistance(sourceCoordinates, destinationCoordinates, DistanceUnits.Miles);
                    user.location.distance = distance;
                    user.location.strDistance = distance.ToString("0.0") + " mi";

                    lstUsers.Add(user);
                }
                UserDistanceComparer userdiscomp = new UserDistanceComparer();
                lstUsers.Sort(userdiscomp.Compare);

                AllBarbers = lstUsers;
            }
        }

        private List<RateImage> getRatingImage(float iCount)
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

        public class UserDistanceComparer : IComparer<User>
        {
            public int Compare(User a, User b)
            {
                if (a.location.distance == b.location.distance)
                    return 0;
                if (a.location.distance < b.location.distance)
                    return -1;

                return 1;
            }
        }
    }
}
