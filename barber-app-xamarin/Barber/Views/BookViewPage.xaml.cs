
using Barber.Models;
using Barber.Services;
using Barber.ViewModels;
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
    public partial class BookViewPage : TabbedPage
    {
        private bool IsInitComponent = true;
        public BookViewPage()
        {
        }

        protected override void OnAppearing()
        {
            if (IsInitComponent)
            {
                InitializeComponent();
                IsInitComponent = false;
            }

            base.OnAppearing();

            BindingContext = new BookViewModel();

            MessagingCenter.Subscribe<BookViewPage>(this, "RefreshView", (sender) =>
            {
                BindingContext = new BookViewModel();
            });
        }

        private async void bookItemClicked(object sender, EventArgs e)
        {
            var stacklayout = (StackLayout)sender;
            Book selectBook = (Book)stacklayout.BindingContext;
            await Navigation.PushAsync(new BookDetailPage(selectBook));
        }
    }
}