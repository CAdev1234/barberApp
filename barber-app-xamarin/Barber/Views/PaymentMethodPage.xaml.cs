using Barber.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Barber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentMethodPage : ContentPage
    {
        public PaymentMethodPage()
        {
            InitializeComponent();
            BindingContext = new PaymentPageViewModel();
        }
    }
}