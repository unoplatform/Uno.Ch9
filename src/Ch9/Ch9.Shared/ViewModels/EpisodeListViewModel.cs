using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using Ch9.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Essentials;

namespace Ch9.ViewModels
{
    public class EpisodeListViewModel : ViewModelBase
    {

        private Show _show;
        public Show Show
        {
            get => _show;
            set => Set(() => Show, ref _show, value);
        }

        private int _episodesCount;
        public int EpisodesCount
        {
            get => _episodesCount;
            set => Set(() => EpisodesCount, ref _episodesCount, value);
        }

        private SourceFeed ShowFeed { get; set; }

        private readonly IShowService _showService;

        public EpisodeListViewModel(SourceFeed showFeed = null)
        {
            ShowFeed = showFeed;

            _showService = SimpleIoc.Default.GetInstance<IShowService>();

            ReloadPage = new RelayCommand(LoadEpisodes);

            DisplaySelectedPost = new RelayCommand<EpisodeViewModel>(post =>
            {
                if (SelectedEpisode != post)
                {
                    SelectedEpisode = post;
                }
            });

            DismissPost = new RelayCommand(() => SelectedEpisode = null);

            SharePost = new RelayCommand<EpisodeViewModel>(async post =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = post.Episode.PostUri.ToString(),
                    Title = post.Episode.Title
                });
            });

            LoadEpisodes();
        }

        public ICommand DisplaySelectedPost { get; }

        public ICommand DismissPost { get; }

        public ICommand ReloadPage { get; }

        public ICommand SharePost { get; }

        private EpisodeViewModel _selectedEpisode;
        public EpisodeViewModel SelectedEpisode
        {
            get => _selectedEpisode;
            set => Set(() => SelectedEpisode, ref _selectedEpisode, value);
        }

        private TaskNotifier<EpisodeViewModel[]> _episodes;
        public TaskNotifier<EpisodeViewModel[]> Episodes
        {
            get => _episodes;
            set => Set(() => Episodes, ref _episodes, value);
        }

        private bool _isVideoFullWindow;
        public bool IsVideoFullWindow
        {
            get => _isVideoFullWindow;

            set
            {
                Set(() => IsVideoFullWindow, ref _isVideoFullWindow, value);

                App.OnFullscreenChanged(value);
            }
        }

        private void LoadEpisodes()
        {
            async Task<EpisodeViewModel[]> GetEpisodes()
            {
                var episodes = await App.ServiceProvider.GetInstance<IEpisodeService>().GetRecentEpisodes(ShowFeed);
                EpisodesCount = episodes.Length;

                var episodesViewModel = episodes.Select(p => new EpisodeViewModel(this, p)).ToArray();

                //Get the current show for the given episode lists
                Show = _showService.GetCurrentShow();

                return episodesViewModel;
            }

            Episodes = new TaskNotifier<EpisodeViewModel[]>(GetEpisodes());
        }
    }
}
