using System;
using System.Collections.Generic;
using System.Text;
using Ch9.Domain;
using GalaSoft.MvvmLight;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ShowPageViewModel : ViewModelBase
    {
        public SourceFeed SourceFeed { get; set; }

        private ShowViewModel _show;
        public ShowViewModel Show
        {
            get => _show;
            set => Set(() => Show, ref _show, value);
        }

        private bool _isNarrowAndSelected;
        public bool IsNarrowAndSelected
        {
            get => _isNarrowAndSelected;
            set => Set(() => IsNarrowAndSelected, ref _isNarrowAndSelected, value);
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
