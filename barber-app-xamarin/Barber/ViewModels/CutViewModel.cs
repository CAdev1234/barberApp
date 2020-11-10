using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Barber.Models;
using Barber.Services;
using Barber.Views;
using Xamarin.Forms;

namespace Barber.ViewModels
{
    public class CutViewModel : BaseViewModel
    {
        public ObservableCollection<Cut> Cuts { get; private set; }
        public IList<Cut> EmptyCuts { get; private set; }

        public Cut PreviousCut { get; set; }
        public Cut CurrentCut { get; set; }
        public Cut CurrentItem { get; set; }
        public int PreviousPosition { get; set; }
        public int CurrentPosition { get; set; }
        public int Position { get; set; }
        private CutViewPage _cutPage = null;

        public ICommand ItemChangedCommand => new Command<Cut>(ItemChanged);
        public ICommand PositionChangedCommand => new Command<int>(PositionChanged);
        public ICommand DeleteCommand => new Command<Cut>(RemoveCut);
        public CutViewModel(CutViewPage page, List<Cut> source, int iPosition)
        {
            _cutPage = page;

            Cuts = new ObservableCollection<Cut>(source);

            CurrentItem = Cuts.Skip(iPosition).FirstOrDefault();
            OnPropertyChanged("CurrentItem");
            Position = iPosition;
            OnPropertyChanged("Position");
        }

        void ItemChanged(Cut item)
        {
            PreviousCut = CurrentCut;
            CurrentCut = item;
            OnPropertyChanged("PreviousCut");
            OnPropertyChanged("CurrentCut");
        }

        void PositionChanged(int position)
        {
            PreviousPosition = CurrentPosition;
            CurrentPosition = position;
            OnPropertyChanged("PreviousPosition");
            OnPropertyChanged("CurrentPosition");
        }

        async void RemoveCut(Cut cut)
        {
            if(Cuts.Count > 0)
            {
                if (await _cutPage.questionDelete())
                {
                    cut = Cuts[CurrentPosition];
                    SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, Cut>(cut, "deletecut");
                    if (res == null)
                        return;

                    Cuts.RemoveAt(CurrentPosition);
                    MessagingCenter.Send<HomePage>(new HomePage(), "RefreshFavouriteBarber");
                }
            }
        }
    }
}
