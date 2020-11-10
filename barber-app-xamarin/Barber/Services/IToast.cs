namespace Barber.Services {
    public interface IToast {
        void Show(string msg, bool longShow = false);
    }
}
