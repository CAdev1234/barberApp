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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookingPage : ContentPage
    {
        private List<Service> m_lstServices = new List<Service>();
        private List<Service> lstCurrentServices = new List<Service>();
        private Image prevPayImg = new Image();
        private int paymentMethod = 0;
        private int currentWeek = -1;
        private string currentTime = "";
        private User currentBarber = null;
        private Button prevButton = null;
        private int currentIndex = -1;
        private PopupPage popupPage = new LoadPopupPage();
        public BookingPage(User user, Service selectedService, List<Service> services)
        {
            InitializeComponent();
            
            m_lstServices = services;

            if (selectedService != null)
            {
                int select = m_lstServices.IndexOf(selectedService);
                m_lstServices[select].image = "check.png";
                m_lstServices[select].check = "1";

                currentIndex = select;

                totalPrice.Text = "$ " + selectedService.price.ToString();
                
                lstCurrentServices.Add(selectedService);
            }

            currentBarber = user;
            prevPayImg = mobilePay;
            drawProfileAsync(user);
         
        }

        private async Task drawProfileAsync(User user)
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberInfoRequest res = await ApiService.GetOneWithPost<BarberInfoRequest, User>(user, "locationinfo");
            if (res == null)
                return;
            
            if (res.Success)
            {
                UserName.Text = user.fullname;
                if(res.Location != null)
                {
                    LocationType.Text = res.Location.city + ", " + res.Location.zipcode;
                    ShopName.Text = res.Location.shopName;
                }
                
                Avatar.Source = user.avatar;

                Services.HeightRequest = m_lstServices.Count * 60;
                Services.ItemsSource = m_lstServices;
            }
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
        }

        private async void serviceItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;
            Service service = (Service)gridItem.BindingContext;
            Label lblCheck = (Label)gridItem.Children[0];
            Image lmgCheck = (Image)gridItem.Children[1];
            if (lblCheck.Text == "0")
            {
                lmgCheck.Source = "check.png";
                lblCheck.Text = "1";
                int index = m_lstServices.IndexOf(service);
                if(currentBarber.booksetting.multi_service == 0 && currentIndex != -1)
                {
                    m_lstServices[currentIndex].image = "uncheck.png";
                    m_lstServices[currentIndex].check = "0";
                    m_lstServices[index].image = "check.png";
                    m_lstServices[index].check = "1";
                    Services.ItemsSource = new List<Service>();
                    Services.ItemsSource = m_lstServices;
                }
                currentIndex = index;
            }
            else if(currentBarber.booksetting.multi_service == 1)
            {
                lmgCheck.Source = "uncheck.png";
                lblCheck.Text = "0";
            }
            if(currentBarber.booksetting.multi_service == 1)
            {
                if (lstCurrentServices.Contains(service))
                    lstCurrentServices.Remove(service);
                else
                    lstCurrentServices.Add(service);
            }
            else
            {
                lstCurrentServices.Clear();
                lstCurrentServices.Add(service);
            }

            int price = 0;
            for(int i = 0; i < lstCurrentServices.Count(); i ++)
                price += lstCurrentServices[i].price;
            totalPrice.Text = "$ " + price.ToString();
        }

        private async void paymentItemClicked(object sender, EventArgs e)
        {
            var gridItem = (Grid)sender;

            Image checkImg = (Image)gridItem.Children[0];
            if (checkImg == prevPayImg)
                return;

            checkImg.Source = "check.png";
            if (prevPayImg != null)
                prevPayImg.Source = "uncheck.png";
            prevPayImg = checkImg;

            if (paymentMethod == 0)
                paymentMethod = 1;
            else
                paymentMethod = 0;
        }

        private async void bookClicked(object sender, EventArgs e)
        {
            if(lstCurrentServices.Count == 0)
            {
                HandleToast.Toast("Service is required.");
                return;
            }
            if (currentTime == "")
            {
                HandleToast.Toast("Time is required.");
                return;
            }

            DateTime datetime = bookDate.Date;
            string strdate = datetime.ToString("yyyy-MM-dd");

            string price = "0";
            for(int i = 0; i < lstCurrentServices.Count; i ++)
                price += lstCurrentServices[i].price;
            
            User client = SharedService.GetUser().User;
            Book newBook = new Book();
            newBook.barber = currentBarber;
            newBook.client = client;
            newBook.services = lstCurrentServices;
            newBook.date = strdate;
            newBook.time = currentTime;
            newBook.payment = paymentMethod;
            newBook.state = 0;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookRequest res = await ApiService.GetOneWithPost<BookRequest, Book>(newBook, "book");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                MessagingCenter.Send<HomePage>(new HomePage(), "RefreshFavouriteBarber");
                MessagingCenter.Send<BarberViewPage>(new BarberViewPage(currentBarber), "RefreshFavouriteBarber");

                newBook.id = res.BookID;

                //auto_comment
                if (newBook.barber.booksetting.auto_comment != null && newBook.barber.booksetting.auto_comment != "")
                {
                    BookComment auto_comment = new BookComment();
                    auto_comment.bookid = newBook.id;
                    auto_comment.comment = newBook.barber.booksetting.auto_comment;
                    auto_comment.senderid = newBook.barber.id;

                    await ApiService.GetOneWithPost<SuccessRequest, BookComment>(auto_comment, "sendcomment");
                }
                Navigation.RemovePage(this);
                Navigation.PushAsync(new BookDetailPage(newBook, true));
            }
        }

        private async void bookDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            currentWeek = (int)bookDate.Date.DayOfWeek;
            string date = bookDate.Date.ToString("yyyy-MM-dd");

            if(bookDate.Date < DateTime.Now.Date)
            {
                HandleToast.Toast("No available date.");
                Times.ItemsSource = new List<Hour>();
                return;
            }

            BarberBookTime booktime = new BarberBookTime();
            booktime.date = date;
            booktime.id = currentBarber.id;
            
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberBookInfo res = await ApiService.GetOneWithPost<BarberBookInfo, BarberBookTime>(booktime, "barberbookinfo");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if(res == null)
                return;

            if (res.Success && res.Location != null)
            {
                List<Hour> lstHours = getServiceTime(res.Location, currentWeek, res.exceptTimes, bookDate.Date == DateTime.Now.Date);
                if(lstHours.Count == 0)
                {
                    HandleToast.Toast("No available time.");
                }
                Times.ItemsSource = lstHours;
            }
        }

        private List<Hour> getServiceTime(LocationInfo location, int week, string[] exceptTimes, bool today)
        {
            string[] lstTimes = null;
            List<Hour> hours = new List<Hour>();
            if(week == 0 && location.sunday != null)
                lstTimes = location.sunday.Split(',');
            else if(week == 1 && location.monday != null)
                lstTimes = location.monday.Split(',');
            else if (week == 2 && location.tuesday != null)
                lstTimes = location.tuesday.Split(',');
            else if (week == 3 && location.wednesday != null)
                lstTimes = location.wednesday.Split(',');
            else if (week == 4 && location.thursday != null)
                lstTimes = location.thursday.Split(',');
            else if (week == 5 && location.friday != null)
                lstTimes = location.friday.Split(',');
            else if (week == 6 && location.saturday != null)
                lstTimes = location.saturday.Split(',');

            if(lstTimes != null)
            {
                for(int i = 0; i < lstTimes.Count(); i++)
                {
                    string[] splitTimes = spliteServiceTime(lstTimes[i], today);
                    for (int j = 0; j < splitTimes.Count(); j ++)
                    {
                        if (exceptTimes.Contains<string>(splitTimes[j]) || splitTimes[j] == null)
                            continue;

                        Hour newHour = new Hour();
                        newHour.hour = splitTimes[j];
                        hours.Add(newHour);
                    }
                }
            }
            return hours;
        }

        private string[] spliteServiceTime(string time, bool today)
        {
            int bookinterval = currentBarber.booksetting.bookinterval;

            string startTime = time.Split('-')[0];
            string startHour = startTime.Split(':')[0];
            string startMin = startTime.Split(':')[1];
            string endTime = time.Split('-')[1];
            string endHour = endTime.Split(':')[0];
            string endMin = endTime.Split(':')[1];

            int iStartTotalMin = int.Parse(startHour) * 60 + int.Parse(startMin);
            int iEndTotalMin = int.Parse(endHour) * 60 + int.Parse(endMin);

            string[] lstTimes = new string[(iEndTotalMin - iStartTotalMin)/ bookinterval];

            int i = 0;
            while(iEndTotalMin >= (iStartTotalMin + bookinterval))
            {
                int hour = iStartTotalMin / 60;
                int min = iStartTotalMin % 60;
                int totalNowMin = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if(today && totalNowMin > (iStartTotalMin - 60))
                {
                    iStartTotalMin += bookinterval;
                    continue;
                }
                string strmin = min < 10 ? "0" + min.ToString() : min.ToString();
                if(hour >= 12)
                {
                    hour -= 12;
                    string strhour = hour < 10 ? "0" + hour.ToString() : hour.ToString();
                    lstTimes[i] = strhour + ":" + strmin + " PM";
                }
                else
                {
                    string strhour = hour < 10 ? "0" + hour.ToString() : hour.ToString();
                    lstTimes[i] = strhour + ":" + strmin + " AM";
                }
                i++;
                iStartTotalMin += bookinterval;
            }

            return lstTimes;
        }

        private void timeSelected(object sender, EventArgs e)
        {
            var button = (Button)sender;
            currentTime = button.Text;
            if (button == prevButton)
                return;

            if (prevButton != null)
            {
                prevButton.TextColor = Color.FromHex("#ddc686");
                prevButton.BackgroundColor = Color.FromHex("#000");
            }
            button.TextColor = Color.FromHex("#000");
            button.BackgroundColor = Color.FromHex("#ddc686");
            prevButton = button;
        }
    }
}