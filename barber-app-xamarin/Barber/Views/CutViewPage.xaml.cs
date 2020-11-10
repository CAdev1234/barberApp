using Barber.Models;
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
    public partial class CutViewPage : ContentPage
    {
        public CutViewPage(List<Cut> lstCuts, int position)
        {
            InitializeComponent();
            BindingContext = new CutViewModel(this, lstCuts, position);
        }
        public async Task<bool> questionDelete()
        {
            return await DisplayAlert("Are you sure?", "Are you sure you want to delete this photo?", "DELETE", "CANCEL");
        }
    }
}