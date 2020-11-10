using System;
using BraintreeCore;
using Foundation;
using UIKit;

namespace Barber.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public const string PayPalUrlScheme = "com.mvpcut.barber.payments";
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            LoadApplication(new App());

            BTAppSwitch.SetReturnURLScheme(PayPalUrlScheme);

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (url.Scheme.Equals(PayPalUrlScheme, StringComparison.InvariantCultureIgnoreCase))
            {
                return BTAppSwitch.HandleOpenURL(url, options: options);
            }

            return false;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (url.Scheme.Equals(PayPalUrlScheme, StringComparison.InvariantCultureIgnoreCase))
            {
                return BTAppSwitch.HandleOpenURL2(url, sourceApplication: sourceApplication);
            }
            return false;
        }
    }
}
