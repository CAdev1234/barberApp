
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
    class BarberBookViewModel : BaseViewModel
    {
        private PopupPage popupPage = new LoadPopupPage();
        private List<Book> _books;
        public List<Book> AllBooks
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }
        public BarberBookViewModel(string day) 
        {
            drawBooks(day);
        }

        public async void drawBooks(string day)
        {
            BarberBook barberbook = new BarberBook();
            barberbook.day = day;

            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BookViewRequest res = await ApiService.GetOneWithPost<BookViewRequest, BarberBook>(barberbook, "barberbooks");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();

            if (res == null)
                return;

            List<string> lstShared = new List<string>();
            lstShared.Add(SharedService.GetValue("request"));
            lstShared.Add(SharedService.GetValue("cancel"));
            lstShared.Add(SharedService.GetValue("confirm"));
            lstShared.Add(SharedService.GetValue("complete"));
            lstShared.Add(SharedService.GetValue("noshow"));
            
            List<int> lstState = new List<int>();
            for(int i = 0; i < lstShared.Count; i++)
            {
                if (lstShared[i] == "0")
                    lstState.Add(i);
            }

            if (res.Success)
            {
                List<Book> lstUpTmp = new List<Book>();
                foreach (Book book in res.Books)
                {
                    if (lstState.Contains(book.state))
                        continue;
                    
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
                    else if (book.state == 2)
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
                    else
                        continue;

                    DateTime bookDate = DateTime.Parse(book.date);
                    book.date = bookDate.ToString("MM/dd/yyyy");

                    book.barber.avatar = ApiService.SERVER_HOST + book.barber.avatar;
                    book.barber.fullname = book.barber.firstname + " " + book.barber.lastname;

                    book.client.avatar = ApiService.SERVER_HOST + book.client.avatar;
                    book.client.fullname = book.client.firstname + " " + book.client.lastname;
                    lstUpTmp.Add(book);
                }
                AllBooks = lstUpTmp;
            }
        }
    }
}