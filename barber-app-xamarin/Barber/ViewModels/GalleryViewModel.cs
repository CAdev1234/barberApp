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
    public class GalleryViewModel : BaseViewModel
    {
        public ObservableCollection<Gallery> Gallerys { get; private set; }
        public IList<Gallery> EmptyGallerys { get; private set; }

        public Gallery PreviousGallery { get; set; }
        public Gallery CurrentGallery { get; set; }
        public Gallery CurrentItem { get; set; }
        public int PreviousPosition { get; set; }
        public int CurrentPosition { get; set; }
        public int Position { get; set; }
        private GalleryViewPage _galleryPage = null;

        public ICommand ItemChangedCommand => new Command<Gallery>(ItemChanged);
        public ICommand PositionChangedCommand => new Command<int>(PositionChanged);
        public ICommand DeleteCommand => new Command<Gallery>(RemoveGallery);
        public GalleryViewModel(GalleryViewPage page, List<Gallery> source, int iPosition)
        {
            _galleryPage = page;

            Gallerys = new ObservableCollection<Gallery>(source);

            CurrentItem = Gallerys.Skip(iPosition).FirstOrDefault();
            OnPropertyChanged("CurrentItem");
            Position = iPosition;
            OnPropertyChanged("Position");
        }

        void ItemChanged(Gallery item)
        {
            PreviousGallery = CurrentGallery;
            CurrentGallery = item;
            OnPropertyChanged("PreviousGallery");
            OnPropertyChanged("CurrentGallery");
        }

        void PositionChanged(int position)
        {
            PreviousPosition = CurrentPosition;
            CurrentPosition = position;
            OnPropertyChanged("PreviousPosition");
            OnPropertyChanged("CurrentPosition");
        }

        async void RemoveGallery(Gallery Gallery)
        {
            if (Gallerys.Count > 0)
            {
                if (await _galleryPage.questionDelete())
                {
                    Gallery = Gallerys[CurrentPosition];

                    SuccessRequest res = await ApiService.GetOneWithPost<SuccessRequest, Gallery>(Gallery, "deleteGallery");
                    if (res == null)
                        return;

                    Gallerys.RemoveAt(CurrentPosition);
                    MessagingCenter.Send<BarberHomePage>(new BarberHomePage(), "RefreshLocation");
                }
            }
        }
    }
}
