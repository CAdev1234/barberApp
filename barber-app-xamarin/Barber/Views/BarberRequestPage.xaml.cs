
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
    public partial class BarberRequestPage : ContentPage
    {
        private PopupPage popupPage = new LoadPopupPage();
        public BarberRequestPage()
        {
            InitializeComponent();
            drawBooks();
        }
        public async void drawBooks()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookViewRequest res = await ApiService.GetOneWithGet<BookViewRequest>("requestedbook");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();

            if (res == null)
                return;

            if (res.Success)
            {
                List<Book> lstUpTmp = new List<Book>();
                foreach (Book book in res.Books)
                {
                    if (book.state != 0)
                        continue;

                    book.allservicename = book.services[0].name;
                    for (int i = 1; i < book.services.Count; i++)
                        book.allservicename += " + " + book.services[1].name;

                    if (book.state == 0)
                    {
                        book.statename = "REQUESTED";
                        book.stateColor = "#ddc686";
                    }

                    book.barber.avatar = ApiService.SERVER_HOST + book.barber.avatar;
                    book.barber.fullname = book.barber.firstname + book.barber.lastname;

                    lstUpTmp.Add(book);
                }

                if(lstUpTmp.Count > 0)
                {
                    RequestedBook.IsVisible = true;
                    NoRequest.IsVisible = false;
                }

                RequestedBook.ItemsSource = lstUpTmp;
            }
        }
        private async void bookItemClicked(object sender, EventArgs e)
        {
            var stacklayout = (StackLayout)sender;
            Book selectBook = (Book)stacklayout.BindingContext;
            await Navigation.PushAsync(new BookDetailPage(selectBook, false));
        }
    }
}