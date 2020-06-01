using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace Ch9.ViewModels
{
    public class ShowPageViewModel : ViewModelBase
    {
        public ShowPageViewModel(Show show)
        {
            CurrentShow = show;
            EpisodesList = new EpisodeListViewModel(show);
        }

        private EpisodeListViewModel _episodesList;
        public EpisodeListViewModel EpisodesList
        {
            get => _episodesList;
            set => Set(() => EpisodesList, ref _episodesList, value);
        }

        private Show _currentShow;
        public Show CurrentShow
        {
            get => _currentShow;
            set => Set(ref _currentShow, value);
        }

    }
}
