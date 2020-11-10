using System;
using System.Threading.Tasks;

using Barber.Services;
using Barber.iOS.Services;
using BraintreeCore;
using BraintreePayPal;
using Foundation;
using PassKit;
using UIKit;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;

[assembly: Dependency(typeof(iOSPayService))]
namespace Barber.iOS.Services
{
    abstract public class iOSPayService : PKPaymentAuthorizationViewControllerDelegate, IPayService
    {
        TaskCompletionSource<string> payTcs;
        public event EventHandler<string> OnTokenizationSuccessful = delegate { };
        public event EventHandler<string> OnTokenizationError = delegate { };

        bool isReady;
        BTAPIClient braintreeClient;

        public bool CanPay
        {
            get
            {
                return isReady;
            }
        }

        public async Task<bool> InitializeAsync(string clientToken)
        {
            var initializeTcs = new TaskCompletionSource<bool>();
            try
            {
                braintreeClient = new BTAPIClient(clientToken);
                isReady = true;
                initializeTcs.TrySetResult(isReady);
            }
            catch (Exception e)
            {
                initializeTcs.TrySetException(e);
            }
            return await initializeTcs.Task;
        }

        public async Task<string> TokenizePayPal()
        {
            payTcs = new TaskCompletionSource<string>();
            if (CanPay)
            {
                var payPalDriver = new BTPayPalDriver(braintreeClient);
                payPalDriver.ViewControllerPresentingDelegate = new BTViewControllerPresenter();
                payPalDriver.AppSwitchDelegate = new BTSwitchDelegate();
                payPalDriver.AuthorizeAccountWithCompletion((BTPayPalAccountNonce payPalAccountNonce, NSError error) =>
                {
                    if (error == null)
                    {
                        if (payPalAccountNonce == null || string.IsNullOrEmpty(payPalAccountNonce.Nonce))
                        {
                            payTcs.SetCanceled();
                        }
                        else
                        {
                            OnTokenizationSuccessful?.Invoke(this, payPalAccountNonce.Nonce);
                            payTcs.TrySetResult(payPalAccountNonce.Nonce);
                        }

                    }
                    else
                    {
                        OnTokenizationError?.Invoke(this, error.Description);
                        payTcs.TrySetException(new Exception(error.Description));
                    }
                });
            }
            else
            {
                OnTokenizationError?.Invoke(this, "Platform is not ready to accept payments");
                payTcs.TrySetException(new Exception("Platform is not ready to accept payments"));

            }

            return await payTcs.Task;
        }
    }

    public class BTSwitchDelegate : BTAppSwitchDelegate
    {
        public override void AppSwitcher(NSObject appSwitcher, BTAppSwitchTarget target)
        {

        }

        public override void AppSwitcherWillPerformAppSwitch(NSObject appSwitcher)
        {
            UserDialogs.Instance.ShowLoading("Loading");
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, HideLoading);
        }

        async void HideLoading(NSNotification obj)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(UIApplication.DidBecomeActiveNotification);
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
        }

        public override void AppSwitcherWillProcessPaymentInfo(NSObject appSwitcher)
        {
            HideLoading(null);
        }
    }


    public class BTViewControllerPresenter : BTViewControllerPresentingDelegate
    {
        public override void RequestsDismissalOfViewController(NSObject driver, UIViewController viewController)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var _viewController = window.RootViewController;
            while (_viewController.PresentedViewController != null)
                _viewController = _viewController.PresentedViewController;

            _viewController?.PresentViewController(viewController, true, null);
        }

        public override void RequestsPresentationOfViewController(NSObject driver, UIViewController viewController)
        {
            viewController.DismissViewController(true, null);
        }
    }

}
