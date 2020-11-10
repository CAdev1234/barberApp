
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
    public partial class EditLocationPage : ContentPage
    {
        private PopupPage popupPage = new LoadPopupPage();
        private int count = 0;
        private List<WEEK> lstWeeks = new List<WEEK>();
        private string locationType = "";
        private enum WEEK
        {
            SUN = 0,
            MON,
            TUE,
            WED,
            THR,
            FRI,
            SAT
        };
        private string[] lstLocationTypes = {"Commercial (Shop, Studio, etc.)", "Resudential (Home, Apartment, etc.)", "Mobile (House-Calls Only)" };
        public EditLocationPage(LocationInfo location)
        {
            InitializeComponent();

            initToggleState(location);
        }

        private void initToggleState(LocationInfo location)
        {
            
            setDefault(location.sunday, sunSwt, sunOpen, sunClose, bsunOpen, bsunClose);
            setDefault(location.monday, monSwt, monOpen, monClose, bmonOpen, bmonClose);
            setDefault(location.tuesday, tueSwt, tueOpen, tueClose, btueOpen, btueClose);
            setDefault(location.wednesday, wedSwt, wedOpen, wedClose, bwedOpen, bwedClose);
            setDefault(location.thursday, thrSwt, thrOpen, thrClose, bthrOpen, bthrClose);
            setDefault(location.friday, friSwt, friOpen, friClose, bfriOpen, bfriClose);
            setDefault(location.saturday, satSwt, satOpen, satClose, bsatOpen, bsatClose);

            setToggleState(WEEK.SUN, sunSwt, sunOpen, sunLine, sunClose, bSunPan);
            setToggleState(WEEK.MON, monSwt, monOpen, monLine, monClose, bMonPan);
            setToggleState(WEEK.TUE, tueSwt, tueOpen, tueLine, tueClose, bTuePan);
            setToggleState(WEEK.WED, wedSwt, wedOpen, wedLine, wedClose, bWedPan);
            setToggleState(WEEK.THR, thrSwt, thrOpen, thrLine, thrClose, bThrPan);
            setToggleState(WEEK.FRI, friSwt, friOpen, friLine, friClose, bFriPan);
            setToggleState(WEEK.SAT, satSwt, satOpen, satLine, satClose, bSatPan);

            pkLocation.SelectedItem = location.locationType;
            shop.Text = location.shopName;
            address.Text = location.streetAddress;
            floor.Text = location.floor;
            city.Text = location.city;
            state.Text = location.state;
            code.Text = location.zipcode;
            country.Text = location.country;
        }

        private void setDefault(string hour, Switch swt, TimePicker tmOpen, TimePicker tmClose, TimePicker tmStart, TimePicker tmEnd)
        {
            if(hour != null)
            {
                swt.IsToggled = true;
                string[] lstHours = hour.Split(',');
                string[] lstFirst = lstHours[0].Split('-');
                string[] lstSecond = lstHours[1].Split('-');
                string open = lstFirst[0];
                string start = lstFirst[1];
                string end = lstSecond[0];
                string close = lstSecond[1];
                tmOpen.Time = TimeSpan.Parse(open);
                tmClose.Time = TimeSpan.Parse(close);
                tmStart.Time = TimeSpan.Parse(start);
                tmEnd.Time = TimeSpan.Parse(end);
                
                count++;
            }
        }

        private void sunSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.SUN, (Switch)sender, sunOpen, sunLine, sunClose, bSunPan);
        }

        private void monSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.MON, (Switch)sender, monOpen, monLine, monClose, bMonPan);
        }

        private void tueSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.TUE, (Switch)sender, tueOpen, tueLine, tueClose, bTuePan);
        }

        private void wedSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.WED, (Switch)sender, wedOpen, wedLine, wedClose, bWedPan);
        }

        private void thrSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.THR, (Switch)sender, thrOpen, thrLine, thrClose, bThrPan);
        }

        private void friSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.FRI, (Switch)sender, friOpen, friLine, friClose, bFriPan);
        }

        private void satSwt_Toggled(object sender, ToggledEventArgs e)
        {
            setToggleState(WEEK.SAT, (Switch)sender, satOpen, satLine, satClose, bSatPan);
        }

        private void setToggleState(WEEK week, Switch swt, TimePicker open, Label line, TimePicker close, StackLayout pan)
        {
            if (swt.IsToggled)
            {
                swt.ThumbColor = Color.FromHex("#ddc686");
                open.TextColor = Color.FromHex("#fff");
                line.TextColor = Color.FromHex("#fff");
                close.TextColor = Color.FromHex("#fff");

                open.IsVisible = true;
                line.IsVisible = true;
                close.IsVisible = true;

                bPanTitle.IsVisible = true;
                pan.IsVisible = true;

                count++;

                lstWeeks.Add(week);
            }
            else
            {
                swt.ThumbColor = Color.FromHex("#555555");
                open.IsVisible = false;
                line.IsVisible = false;
                close.IsVisible = false;

                pan.IsVisible = false;
             
                count--;
                if(count == 0)
                    bPanTitle.IsVisible = false;
                if(lstWeeks.Count > 0)
                    lstWeeks.Remove(week);
            }
        }

        private async void save(object sender, EventArgs e)
        {
            LocationInfo location = new LocationInfo();

            string strshop = shop.Text;
            string straddress = address.Text;
            string strfloor = floor.Text;
            string strcity = city.Text;
            string strstate = state.Text;
            string strcode = code.Text;
            string strcountry = country.Text;

            if(locationType == "")
            {
                HandleToast.Toast("Invalid LocationType.");
                return;
            }
            if(strshop == null || strshop == "")
            {
                HandleToast.Toast("Invalid ShopName.");
                return;
            }
            if (straddress == null || straddress == "")
            {
                HandleToast.Toast("Invalid Street Address.");
                return;
            }
            if (strfloor == null || strfloor == "")
            {
                HandleToast.Toast("Invalid Floor.");
                return;
            }
            if (strcity == null || strcity == "")
            {
                HandleToast.Toast("Invalid City.");
                return;
            }
            if (strstate == null || strstate == "")
            {
                HandleToast.Toast("Invalid State.");
                return;
            }
            if (strcountry == null || strcountry == "")
            {
                HandleToast.Toast("Invalid Country.");
                return;
            }
            if (strcode == null || strcode == "")
            {
                HandleToast.Toast("Invalid Zip Code.");
                return;
            }

            location.locationType = locationType;
            location.shopName = strshop;
            location.streetAddress = straddress;
            location.floor = strfloor;
            location.city = strcity;
            location.state = strstate;
            location.country = strcountry;
            location.zipcode = strcode;

            try
            {
                var currentlocation = (await Geocoding.GetLocationsAsync($"{straddress}, {strcity}, {strcountry}")).FirstOrDefault();
                location.latitude = currentlocation.Latitude;
                location.longitude = currentlocation.Longitude;
            }
            catch(Exception error)
            {
                string message = error.Message;
                HandleToast.Toast("Cannot find your position. Please sure your network connection.");
                return;
            }
            
            if (lstWeeks.Contains(WEEK.SUN))
            {
                if(!isValid(sunOpen.Time, sunClose.Time, bsunOpen.Time, bsunClose.Time))
                {
                    HandleToast.Toast("Invalid Sunday time.");
                    return;
                }
                location.sunday = getValidTime(sunOpen.Time, sunClose.Time, bsunOpen.Time, bsunClose.Time);
            }
            else
                location.sunday = "";

            if (lstWeeks.Contains(WEEK.MON))
            {
                if (!isValid(monOpen.Time, monClose.Time, bmonOpen.Time, bmonClose.Time))
                {
                    HandleToast.Toast("Invalid Monday time.");
                    return;
                }
                location.monday = getValidTime(monOpen.Time, monClose.Time, bmonOpen.Time, bmonClose.Time);
            }
            else
                location.monday = "";

            if (lstWeeks.Contains(WEEK.TUE))
            {
                if (!isValid(tueOpen.Time, tueClose.Time, btueOpen.Time, btueClose.Time))
                {
                    HandleToast.Toast("Invalid Tuesday time.");
                    return;
                }
                location.tuesday = getValidTime(tueOpen.Time, tueClose.Time, btueOpen.Time, btueClose.Time);
            }
            else
                location.tuesday = "";

            if (lstWeeks.Contains(WEEK.WED))
            {
                if (!isValid(wedOpen.Time, wedClose.Time, bwedOpen.Time, bwedClose.Time))
                {
                    HandleToast.Toast("Invalid Wednesday time.");
                    return;
                }
                location.wednesday = getValidTime(wedOpen.Time, wedClose.Time, bwedOpen.Time, bwedClose.Time);
            }
            else
                location.wednesday = "";

            if (lstWeeks.Contains(WEEK.THR))
            { 
                if (!isValid(thrOpen.Time, thrClose.Time, bthrOpen.Time, bthrClose.Time))
                {
                    HandleToast.Toast("Invalid Thursday time.");
                    return;
                }
                location.thursday = getValidTime(thrOpen.Time, thrClose.Time, bthrOpen.Time, bthrClose.Time);
            }
            else
                location.thursday = "";

            if(lstWeeks.Contains(WEEK.FRI))
            {
                if (!isValid(friOpen.Time, friClose.Time, bfriOpen.Time, bfriClose.Time))
                {
                    HandleToast.Toast("Invalid Friday time.");
                    return;
                }
                location.friday = getValidTime(friOpen.Time, friClose.Time, bfriOpen.Time, bfriClose.Time);
            }
            else
                location.friday = "";

            if (lstWeeks.Contains(WEEK.SAT))
            {
                if (!isValid(satOpen.Time, satClose.Time, bsatOpen.Time, bsatClose.Time))
                {
                    HandleToast.Toast("Invalid Saturday time.");
                    return;
                }
                location.saturday = getValidTime(satOpen.Time, satClose.Time, bsatOpen.Time, bsatClose.Time);
            }
            else
                location.saturday = "";

            location.barberid = SharedService.GetUser().User.id;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, LocationInfo>(location, "savelocation");
            if (res == null)
                return;

            if (res.Success)
            {
                MessagingCenter.Send<BarberHomePage>(new BarberHomePage(), "RefreshLocation");
                Navigation.PopAsync(true);
            }
        }
        private string getValidTime(TimeSpan open, TimeSpan close, TimeSpan start, TimeSpan end)
        {
            string stropen = open.ToString("hh") + ":" + open.ToString("mm");
            string strclose = close.ToString("hh") + ":" + close.ToString("mm");
            string strstart = start.ToString("hh") + ":" + start.ToString("mm");
            string strend = end.ToString("hh") + ":" + end.ToString("mm");
            string totalTIme = stropen + "-" + strstart + "," + strend + "-" + strclose;
            return totalTIme;
        }
        private bool isValid(TimeSpan open, TimeSpan close, TimeSpan start, TimeSpan end)
        {
            if((open >= close) || (start >= end))
                return false;
            if ((open >= start) || (close <= end))
                return false;
            
            return true;
        }

        private void pkLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            locationType = lstLocationTypes[pkLocation.SelectedIndex];
        }
    }
}