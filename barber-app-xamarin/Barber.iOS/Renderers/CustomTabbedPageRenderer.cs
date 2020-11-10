using UIKit;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Barber.iOS.Renderers;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Barber.iOS.Renderers
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        private TabbedPage _page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
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

            try
            {
                var tabbarController = (UITabBarController)this.ViewController;
                if (null != tabbarController)
                {
                    tabbarController.ViewControllerSelected += OnTabbarControllerItemSelected;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void OnTabbarControllerItemSelected(object sender, UITabBarSelectionEventArgs eventArgs)
        {
            await _page.CurrentPage.Navigation.PopToRootAsync();
        }
    }
}