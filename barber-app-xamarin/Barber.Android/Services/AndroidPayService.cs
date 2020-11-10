using System;
using System.Threading.Tasks;
using Android.Runtime;
using Barber.Services;
using Barber.Droid.Services;
using Com.Braintreepayments.Api;
using Com.Braintreepayments.Api.Exceptions;
using Com.Braintreepayments.Api.Interfaces;
using Com.Braintreepayments.Api.Models;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPayService))]
namespace Barber.Droid.Services
{
    public class AndroidPayService : Java.Lang.Object, IPayService, IBraintreeResponseListener, IBraintreeErrorListener, IPaymentMethodNonceCreatedListener, IBraintreeCancelListener
    {
        TaskCompletionSource<bool> initializeTcs;
        TaskCompletionSource<string> payTcs;

        BraintreeFragment mBraintreeFragment;
        bool isReady = false;
        public bool CanPay { get { return isReady; } }

        public event EventHandler<string> OnTokenizationSuccessful;
        public event EventHandler<string> OnTokenizationError;

        public void OnCancel(int requestCode)
        {
            payTcs.SetCanceled();
            mBraintreeFragment.RemoveListener(this);
        }

        public void OnResponse(Java.Lang.Object parameter)
        {
            if (parameter is Java.Lang.Boolean)
            {
                var res = parameter.JavaCast<Java.Lang.Boolean>();
                isReady = res.BooleanValue();
                initializeTcs?.TrySetResult(res.BooleanValue());
            }
        }

        public void OnPaymentMethodNonceCreated(PaymentMethodNonce paymentMethodNonce)
        {
            // Send this nonce to your server
            string nonce = paymentMethodNonce.Nonce;
            mBraintreeFragment.RemoveListener(this);
            OnTokenizationSuccessful?.Invoke(this, nonce);
            payTcs?.TrySetResult(nonce);
        }

        public void OnError(Java.Lang.Exception error)
        {
            if (error is ErrorWithResponse)
            {
                ErrorWithResponse errorWithResponse = (ErrorWithResponse)error;
                BraintreeError cardErrors = errorWithResponse.ErrorFor("creditCard");
                if (cardErrors != null)
                {
                    BraintreeError expirationMonthError = cardErrors.ErrorFor("expirationMonth");
                    if (expirationMonthError != null)
                    {
                        OnTokenizationError?.Invoke(this, expirationMonthError.Message);
                        payTcs?.TrySetException(new System.Exception(expirationMonthError.Message));

                    }
                    else
                    {
                        OnTokenizationError?.Invoke(this, cardErrors.Message);
                        payTcs?.TrySetException(new System.Exception(cardErrors.Message));

                    }
                }
            }
            mBraintreeFragment.RemoveListener(this);
        }


        public async Task<string> TokenizePayPal()
        {
            payTcs = new TaskCompletionSource<string>();
            if (isReady)
            {
                mBraintreeFragment.AddListener(this);
                PayPal.AuthorizeAccount(mBraintreeFragment);
            }
            else
            {
                OnTokenizationError?.Invoke(this, "Platform is not ready to accept payments");
                payTcs.TrySetException(new System.Exception("Platform is not ready to accept payments"));

            }
            return await payTcs.Task;
        }


        public async Task<bool> InitializeAsync(string clientToken)
        {
            try
            {
                initializeTcs = new TaskCompletionSource<bool>();
                mBraintreeFragment = BraintreeFragment.NewInstance(CrossCurrentActivity.Current.Activity, clientToken);

                GooglePayment.IsReadyToPay(mBraintreeFragment, this);
            }
            catch (InvalidArgumentException e)
            {
                initializeTcs.TrySetException(e);
            }
            return await initializeTcs.Task;
        }

    }
}
