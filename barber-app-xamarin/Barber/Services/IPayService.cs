using System;
using System.Threading.Tasks;

namespace Barber.Services
{
    public interface IPayService
    {
        event EventHandler<string> OnTokenizationSuccessful;

        event EventHandler<string> OnTokenizationError;

        bool CanPay { get; }

        Task<bool> InitializeAsync(string clientToken);

        Task<string> TokenizePayPal();
    }
}
