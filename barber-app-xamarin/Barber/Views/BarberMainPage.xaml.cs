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
    public partial class BarberMainPage : TabbedPage
    {
        public BarberMainPage()
        {
            InitializeComponent();
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            var page = CurrentPage as NavigationPage;
            page.Navigation.PopToRootAsync();
        }
    }
}