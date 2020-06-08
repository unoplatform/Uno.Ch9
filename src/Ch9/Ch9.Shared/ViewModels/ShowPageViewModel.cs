using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Ch9.ViewModels
{
    public class ShowPageViewModel : ViewModelBase
    {
        public SourceFeed SourceFeed { get; set; }

        private ShowViewModel _show;
        public ShowViewModel Show
        {
            get => _show;
            set => Set(() => Show, ref _show, value);
        }

        public void OnNavigatedTo(SourceFeed sourceFeed)
        {
            if (Show == null)
            {
                SourceFeed = sourceFeed;

                Show = new ShowViewModel(SourceFeed);
            }
        }
    }
}
