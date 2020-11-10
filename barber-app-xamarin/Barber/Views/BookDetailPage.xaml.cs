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
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookDetailPage : ContentPage
    {
        public Book currentBook;
        public bool m_bEdit = false;
        private PopupPage popupPage = new LoadPopupPage();
        private enum BookState
        {
            REQUESTED = 0,
            CANCELLED,
            CONFIRMED,
            COMPLETED,
            DECLINED,
            NOSHOW
        };
        public BookDetailPage(Book book, bool edit = false)
        {
            InitializeComponent();

            m_bEdit = edit;
            currentBook = book;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            btnDone.IsVisible = false;
            if (m_bEdit)
            {
                btnDone.IsVisible = true;
                export.IsVisible = true;
            }
            infophonepan.IsVisible = false;

            if (SharedService.GetUser().User.barber)
            {
                if (currentBook.state == (int)BookState.REQUESTED)
                {
                    imgaction1.Source = "confirm.png";
                    lblaction1.Text = "CONFIRM";
                    imgaction2.Source = "decline.png";
                    lblaction2.Text = "DECLINE";
                    lblaction2.TextColor = Color.FromHex("db6e53");
                }
                else if (currentBook.state == (int)BookState.CONFIRMED)
                {
                    imgaction1.Source = "checkout.png";
                    lblaction1.Text = "CHECKOUT";
                    lblaction1.TextColor = Color.FromHex("00b894");
                    imgaction2.Source = "cancel.png";
                    lblaction2.Text = "CANCEL";
                    lblaction2.TextColor = Color.FromHex("db6e53");
                    action3.IsVisible = true;
                }
                else
                {
                    actionPan.IsVisible = false;
                }
            }
            else
            {
                if (currentBook.state == (int)BookState.REQUESTED)
                {
                    imgaction1.Source = "cancel.png";
                    lblaction1.Text = "CANCEL";
                    lblaction1.TextColor = Color.FromHex("db6e53");
                    imgaction2.Source = "share.png";
                    lblaction2.Text = "SHARE";
                }
                else
                {
                    actionPan.IsVisible = false;
                }
            }

            drawUserInfoAsync(currentBook);

            if (currentBook.bookcomments == null)
                return;

            foreach (BookComment item in currentBook.bookcomments)
            {
                if(SharedService.GetUser().User.id == item.senderid)
                {
                    item.sender = SharedService.GetUser().User;
                    item.isSend = true;
                    item.isReceive = false;
                }
                else
                {
                    if (item.senderid == currentBook.barber.id)
                        item.sender = currentBook.barber;
                    else
                        item.sender = currentBook.client;
                    item.isSend = false;
                    item.isReceive = true;
                }
                item.created_at = item.created_at.Split('T')[0];

                DateTime formatDate = DateTime.Parse(item.created_at);
                item.created_at = formatDate.ToString("MM/dd/yyyy");
            }

            if (currentBook.bookcomments.Count > 0)
                commentPan.IsVisible = true;

            BookComments.ItemsSource = currentBook.bookcomments;
        }
        private async Task drawUserInfoAsync(Book book)
        {
            User user = book.barber;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberInfoRequest res = await ApiService.GetOneWithPost<BarberInfoRequest, User>(user, "locationinfo");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                if (SharedService.GetUser().User.barber)
                {
                    Avatar.Source = book.client.avatar;
                    barbername.Text = book.client.fullname;
                    ClientInfoPan.IsVisible = false;
                }
                else
                {
                    Avatar.Source = book.barber.avatar;
                    barbername.Text = book.barber.fullname;
                    if (res.Location != null)
                    {
                        location.Text = res.Location.city + ", " + res.Location.streetAddress + ", " + res.Location.zipcode;
                    }
                }

                DateTime bookDate = DateTime.Parse(book.date);
                book.date = bookDate.ToString("MM/dd/yyyy");

                exDate.Text = book.date;
                exTime.Text = book.time;

                proDate.Text = book.date;
                if (book.state == (int)BookState.REQUESTED)
                {
                    state.Text = "REQUESTED";
                    state.TextColor = Color.FromHex("#000");
                    frState.BackgroundColor = Color.FromHex("#ddc686");
                }
                else if (book.state == (int)BookState.CANCELLED)
                {
                    state.Text = "CANCELLED";
                    state.TextColor = Color.FromHex("#000");
                    frState.BackgroundColor = Color.FromHex("#db6e53");
                }
                else if (book.state == (int)BookState.DECLINED)
                {
                    state.Text = "DECLINED";
                    state.TextColor = Color.FromHex("#000");
                    frState.BackgroundColor = Color.FromHex("#db6e53");
                }
                else if (book.state == (int)BookState.CONFIRMED)
                {
                    state.Text = "CONFIRMED";
                    state.TextColor = Color.FromHex("#000");
                    frState.BackgroundColor = Color.FromHex("#0dd6f7");
                }
                else if (book.state == (int)BookState.COMPLETED)
                {
                    state.Text = "COMPLETED";
                    state.TextColor = Color.FromHex("#000");
                    frState.BackgroundColor = Color.FromHex("#51f70d");
                }
                string strservice = book.services[0].name;
                string strallname = book.services[0].name;
                for (int i = 1; i < book.services.Count; i++)
                {
                    strservice += " + " + book.services[1].name;
                    strallname += "\n" + book.services[1].name;
                }
                if (!SharedService.GetUser().User.barber)
                    services.Text = strservice;
                allCount.Text = (book.services.Count).ToString() + " Services";
                allName.Text = strallname;

                int totalprice = 0;
                for (int i = 0; i < book.services.Count; i++)
                    totalprice += book.services[i].price;

                price.Text = "$" + totalprice.ToString();
            }
            mainContent.IsVisible = true;
        }

        private async void done_Clicked(object sender, EventArgs e)
        {
            HandleToast.Toast("This appointment is requested.");
            MessagingCenter.Send<BookViewPage>(new BookViewPage(), "RefreshView");
            Navigation.PopAsync(true);
            btnDone.IsVisible = false;
            actionPan.IsVisible = false;
        }

        private async void action1_Clicked(object sender, EventArgs e)
        {
            if (SharedService.GetUser().User.barber)
            {
                if (currentBook.state == (int)BookState.CONFIRMED)
                    currentBook.state = (int)BookState.COMPLETED;
                else
                    currentBook.state = (int)BookState.CONFIRMED;
            }
            else
            {
                currentBook.state = (int)BookState.CANCELLED;
            }
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookStateRequest res = await ApiService.GetOneWithPost<BookStateRequest, Book>(currentBook, "statebook");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                string message = "";
                if (currentBook.state == (int)BookState.CONFIRMED)
                {
                    message = "confirmed.";
                    state.Text = "CONFIRMED";
                    frState.BackgroundColor = Color.FromHex("#0dd6f7");

                    actionPan.IsVisible = true;
                    imgaction1.Source = "confirm.png";
                    lblaction1.Text = "CHECKOUT";
                    imgaction2.Source = "cancel.png";
                    lblaction2.Text = "CANCEL";
                    action3.IsVisible = true;
                }
                else if (currentBook.state == (int)BookState.COMPLETED)
                {
                    message = "completed.";
                    state.Text = "COMPLETED";
                    frState.BackgroundColor = Color.FromHex("#51f70d");
                    actionPan.IsVisible = false;
                }
                else if (currentBook.state == (int)BookState.CANCELLED)
                {
                    message = "cancelled.";
                    state.Text = "CANCELLED";
                    frState.BackgroundColor = Color.FromHex("#db6e53");
                    actionPan.IsVisible = false;
                }
                else if (currentBook.state == (int)BookState.DECLINED)
                {
                    message = "declined.";
                    state.Text = "DECLINED";
                    frState.BackgroundColor = Color.FromHex("#db6e53");
                    actionPan.IsVisible = false;
                }

                btnDone.IsVisible = false;

                HandleToast.Toast("This appointment is " + message);

                MessagingCenter.Send<BookViewPage>(new BookViewPage(), "RefreshView");
                MessagingCenter.Send<BarberBookPage>(new BarberBookPage(), "RefreshBook");
                MessagingCenter.Send<NotificationPage>(new NotificationPage(), "RefreshView");
            }
        }

        private async void action2_Clicked(object sender, EventArgs e)
        {
            if (!SharedService.GetUser().User.barber)
                return;

            if(!SharedService.GetUser().User.barber && currentBook.state == (int)BookState.REQUESTED)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = "You should definitely check out this app, theCut. See if your barber is on it, or find other barbers in your area. You can schedule your next haircut in advance, pay and tip all through the app. No more waiting in the shop. It's a game changer. https://book.thecut.co/client-recommend",
                    Title = "Share With"
                });
                return;
            }

            if (SharedService.GetUser().User.barber && currentBook.state != (int)BookState.CONFIRMED)
                currentBook.state = (int)BookState.DECLINED;
            else
                currentBook.state = (int)BookState.CANCELLED;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookStateRequest res = await ApiService.GetOneWithPost<BookStateRequest, Book>(currentBook, "statebook");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                if (currentBook.state == (int)BookState.CANCELLED)
                {
                    state.Text = "CANCELLED";
                    HandleToast.Toast("This appointment is cancelled.");
                }
                else
                {
                    state.Text = "DECLINED";
                    HandleToast.Toast("This appointment is declined.");
                }
                state.TextColor = Color.FromHex("#000");
                frState.BackgroundColor = Color.FromHex("#db6e53");

                actionPan.IsVisible = false;

                MessagingCenter.Send<BookViewPage>(new BookViewPage(), "RefreshView");
                MessagingCenter.Send<BarberBookPage>(new BarberBookPage(), "RefreshBook");
                MessagingCenter.Send<NotificationPage>(new NotificationPage(), "RefreshView");
            }
        }
        private async void action3_Clicked(object sender, EventArgs e)
        {

        }

        private void exportDate(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://calendar.google.com/"));
        }
        private void directionInfo(object sender, EventArgs e)
        {

        }

        private async void sendComment(object sender, EventArgs e)
        {
            string strComment = Comment.Text;
            if (strComment == null || strComment == "")
                return;

            BookComment comment = new BookComment();
            comment.comment = strComment;
            comment.bookid = currentBook.id;
            comment.senderid = SharedService.GetUser().User.id;
            comment.isReceive = false;
            comment.isSend = true;
            comment.sender = SharedService.GetUser().User;
            comment.created_at = DateTime.Now.ToString("yyyy-MM-dd");

            SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, BookComment>(comment, "sendcomment");
            if (res == null)
                return;

            commentPan.IsVisible = true;

            if (currentBook.bookcomments == null)
                currentBook.bookcomments = new List<BookComment>();

            currentBook.bookcomments.Add(comment);
            BookComments.ItemsSource = new List<BookComment>();
            BookComments.ItemsSource = currentBook.bookcomments;

            Comment.Text = "";
        }

        private async void callPhone(object sender, EventArgs e)
        {
            try
            {
                PhoneDialer.Open(infophone.Text);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
                HandleToast.Toast("Phone Dialer is not supported on this device.");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private async void sendSms(object sender, EventArgs e)
        {
            try
            {
                var message = new SmsMessage("", new[] { infophone.Text });
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
                HandleToast.Toast("Sms is not supported on this device.");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

    }
}