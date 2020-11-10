
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Barber.ViewModels
{
    class BookViewModel : BaseViewModel
    {
        private List<Book> _upbooks;
        private List<Book> _pastbooks;
        private PopupPage popupPage = new LoadPopupPage();
        public List<Book> UpcomingBooks
        {
            get => _upbooks;
            set => SetProperty(ref _upbooks, value);
        }
        public List<Book> PastBooks
        {
            get => _pastbooks;
            set => SetProperty(ref _pastbooks, value);
        }
        public BookViewModel()
        {
            drawBooks();
        }

        public async void drawBooks()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookViewRequest res = await ApiService.GetOneWithGet<BookViewRequest>("clientbooks");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();

            if (res == null)
                return;

            if (res.Success)
            {
                List<Book> lstUpTmp = new List<Book>();
                List<Book> lstPastTmp = new List<Book>();
                foreach (Book book in res.Books)
                {
                    book.allservicename = book.services[0].name;
                    for (int i = 1; i < book.services.Count; i++)
                        book.allservicename += " + " + book.services[1].name;

                    if (book.state == 0)
                    {
                        book.statename = "REQUESTED";
                        book.stateColor = "#ddc686";
                    }
                    else if (book.state == 1)
                    {
                        book.statename = "CANCELLED";
                        book.stateColor = "#db6e53";
                    }
                    else if(book.state == 2)
                    {
                        book.statename = "CONFIRMED";
                        book.stateColor = "#0dd6f7";
                    }
                    else if (book.state == 3)
                    {
                        book.statename = "COMPLETED";
                        book.stateColor = "#51f70d";
                    }
                    else if (book.state == 4)
                    {
                        book.statename = "DECLINED";
                        book.stateColor = "#db6e53";
                    }

                    DateTime bookDate = DateTime.Parse(book.date);
                    book.date = bookDate.ToString("MM/dd/yyyy");

                    book.barber.avatar = ApiService.SERVER_HOST + book.barber.avatar;
                    book.barber.fullname = book.barber.firstname + " " + book.barber.lastname;

                    bookDate = Convert.ToDateTime(book.date);
                    int compare = DateTime.Compare(DateTime.Now, bookDate);
                    if (compare >= 0)
                        lstPastTmp.Add(book);
                    else
                        lstUpTmp.Add(book);
                }
                UpcomingBooks = lstUpTmp;
                PastBooks = lstPastTmp;
            }
        }
    }
}