using System;
using System.Collections.Generic;
using System.Text;
using Ch9.Domain;
using Ch9.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Ch9.ViewModels
{
    public class ShowPageViewModel : ViewModelBase
    {
        public ShowPageViewModel(SourceFeed showFeed)
        {
            EpisodesList = new EpisodeListViewModel(showFeed);
        }

        private EpisodeListViewModel _episodesList;
        public EpisodeListViewModel EpisodesList
        {
            get => _episodesList;
            set => Set(() => EpisodesList, ref _episodesList, value);
        }
    }
}
