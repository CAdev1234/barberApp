
using Barber.Models;
using Barber.Popups;
using Barber.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barber.ViewModels
{
    class HomeViewModel : BaseViewModel
    {
        private PopupPage popupPage = new LoadPopupPage();
        private List<User> _barbers;
        private List<Cut> _cuts;
        public List<User> MyBarbers
        {
            get => _barbers;
            set => SetProperty(ref _barbers, value);
        }
        public List<Cut> MyCuts
        {
            get => _cuts;
            set => SetProperty(ref _cuts, value);
        }
        public HomeViewModel()
        {
            drawBarbersAsync();
        }
        public async Task drawBarbersAsync()
        {
            if (PopupNavigation.PopupStack.Count == 0) await PopupNavigation.Instance.PushAsync(popupPage);
            BarberModel res = await ApiService.GetOneWithGet<BarberModel>("barber");
            if (PopupNavigation.PopupStack.Count > 0) await PopupNavigation.Instance.PopAllAsync();
            if (res == null)
                return;

            if (res.Success)
            {
                foreach (User item in res.Users)
                {
                    item.avatar = ApiService.SERVER_HOST + item.avatar;
                    item.fullname = item.firstname + " " + item.lastname;
                }
                MyBarbers = new List<User>();
                MyBarbers = res.Users;
                List<Cut> tmpCuts = new List<Cut>();
                foreach (Cut item in res.Cuts)
                {
                    item.sz_cuts = ApiService.SERVER_HOST + item.sz_cuts;
                    tmpCuts.Add(item);
                }
                if(tmpCuts.Count == 0)
                {
                    Cut addItem = new Cut();
                    addItem.id = -1;
                    addItem.n_id_user = -1;
                    addItem.sz_cuts = "addPhoto.png";
                    tmpCuts.Add(addItem);
                }
                
                MyCuts = tmpCuts;
            }
        }

        public async Task addCutPhoto(string filepath)
        {
            CutRequest res = await ApiService.uploadPhotoWithPost<CutRequest>("addcut", filepath);
            if (res == null)
                return;

            if (res.Success)
            {
                if (MyCuts[0].id == -1)
                    MyCuts.Clear();

                res.Cut.sz_cuts = ApiService.SERVER_HOST + res.Cut.sz_cuts;

                List<Cut> lstCuts = new List<Cut>();
                lstCuts = MyCuts;
                lstCuts.Add(res.Cut);
                MyCuts = lstCuts;

                drawBarbersAsync();
            }
        }

        public List<Cut> getMyCuts()
        {
            return MyCuts;
        }

        public List<User> getMyBarbers()
        {
            return MyBarbers;
        }
    }
}
