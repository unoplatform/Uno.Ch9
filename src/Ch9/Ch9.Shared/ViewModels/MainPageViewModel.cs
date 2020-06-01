using System.Collections.Generic;
using Ch9.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Services;
using Xamarin.Essentials;

namespace Ch9
{
    [Windows.UI.Xaml.Data.Bindable]
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            ToAboutPage = new RelayCommand(() =>
            {
                App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(AboutPage));
            });

            Shows = App.ServiceProvider.GetInstance<IShowService>().GetShows();

            DisplayShow = new RelayCommand<Show>(show  =>
            {
                App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(ShowPage), show);
            });
        }

        public ICommand ToAboutPage { get; }

        public ICommand DisplayShow { get; set; }

        public ICollection<Show> Shows { get; set; }

        private EpisodeListViewModel _episodesList;
        public EpisodeListViewModel EpisodesList
        {
            get => _episodesList;
            set => Set(() => EpisodesList, ref _episodesList, value);
        }

        public void OnNavigatedTo()
        {
            if (EpisodesList == null)
            {
                EpisodesList = new EpisodeListViewModel();
            }
        }
    }
}
