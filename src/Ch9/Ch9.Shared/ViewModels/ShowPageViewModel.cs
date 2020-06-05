using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Ch9.Domain;
using Ch9.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Ch9.ViewModels
{
    public class ShowPageViewModel : ViewModelBase
    {
        public ShowPageViewModel(SourceFeed showFeed)
        {
            EpisodesList = new EpisodeListViewModel(showFeed);

            ToAboutPage = new RelayCommand(() =>
            {
                App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(AboutPage));
            });
        }

        public ICommand ToAboutPage { get; }

        private EpisodeListViewModel _episodesList;
        public EpisodeListViewModel EpisodesList
        {
            get => _episodesList;
            set => Set(() => EpisodesList, ref _episodesList, value);
        }
    }
}
