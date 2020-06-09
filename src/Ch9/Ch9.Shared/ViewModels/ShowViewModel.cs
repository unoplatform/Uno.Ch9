using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xamarin.Essentials;

namespace Ch9.ViewModels
{
    public class ShowViewModel : ViewModelBase
    {
        private SourceFeed _sourceFeed;

        public ShowViewModel(SourceFeed sourceFeed = null)
        {
            _sourceFeed = sourceFeed;

            ReloadPage = new RelayCommand(LoadShow);

            DisplaySelectedEpisode = new RelayCommand<EpisodeViewModel>(episode =>
            {
                if (SelectedEpisode != episode)
                {
                    SelectedEpisode = episode;
                }
            });

            DismissSelectedEpisode = new RelayCommand(() => SelectedEpisode = null);

            ShareEpisode = new RelayCommand<EpisodeViewModel>(async episode =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = episode.Episode.EpisodeUri.ToString(),
                    Title = episode.Episode.Title
                });
            });

            LoadShow();
        }

        public ICommand DisplaySelectedEpisode { get; }

        public ICommand DismissSelectedEpisode { get; }

        public ICommand ReloadPage { get; }

        public ICommand ShareEpisode { get; }

        private TaskNotifier<Show> _show;
        public TaskNotifier<Show> Show
        {
            get => _show;
            set => Set(() => Show, ref _show, value);
        }

        private EpisodeViewModel[] _episodes;
        public EpisodeViewModel[] Episodes
        {
            get => _episodes;
            set => Set(() => Episodes, ref _episodes, value);
        }

        private EpisodeViewModel _selectedEpisode;
        public EpisodeViewModel SelectedEpisode
        {
            get => _selectedEpisode;
            set => Set(() => SelectedEpisode, ref _selectedEpisode, value);
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

        private void LoadShow()
        {
            async Task<Show> GetShow()
            {
                var show = await Task.Run(async () =>
                {
                    return await App.ServiceProvider.GetInstance<IShowService>().GetShow(_sourceFeed);
                });

                Episodes = show.Episodes.Select(p => new EpisodeViewModel(this, p)).ToArray();

                return show;
            }

            Show = new TaskNotifier<Show>(GetShow());
        }
    }
}
