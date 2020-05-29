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
            Episodes = new EpisodeListViewModel(show);
        }

        private EpisodeListViewModel _episodes;
        public EpisodeListViewModel Episodes
        {
            get => _episodes;
            set => Set(() => Episodes, ref _episodes, value);
        }

    }
}
