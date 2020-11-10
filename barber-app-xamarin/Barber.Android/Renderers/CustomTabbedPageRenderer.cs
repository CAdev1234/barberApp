using Barber.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Google.Android.Material.BottomNavigation;
using Android.Views;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Barber.Renderers
{
    public class CustomTabbedPageRenderer : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private TabbedPage _page;
        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                _page = (TabbedPage)e.NewElement;
            }
            else
            {
                _page = (TabbedPage)e.OldElement;
            }
        }
        /*async void TabLayout.IOnTabSelectedListener.OnTabReselected(TabLayout.Tab tab)
        {
            //inplement TabLayout.IOnTabSelectedListener
            //when tabtoolbar position in top
            await _page.CurrentPage.Navigation.PopToRootAsync();
        }*/
        bool BottomNavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
        {
            //when tabtoolbar position in bottom

            base.OnNavigationItemSelected(item);

            _page.CurrentPage.Navigation.PopToRootAsync();

            return true;
        }
    }
}