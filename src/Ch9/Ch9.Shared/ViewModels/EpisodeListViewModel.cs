using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xamarin.Essentials;

namespace Ch9.ViewModels
{
    public class EpisodeListViewModel : ViewModelBase
    {

        public Show Show { get; set; }

        public EpisodeListViewModel(Show show = null)
        {
            Show = show;

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

        public ICommand ToSelectedShowPage { get; }

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
                var episodes = await App.ServiceProvider.GetInstance<IEpisodeService>().GetRecentEpisodes(Show);

                var episodesViewModel = episodes.Select(p => new EpisodeViewModel(this, p)).ToArray();

                return episodesViewModel;
            }

            Episodes = new TaskNotifier<EpisodeViewModel[]>(GetEpisodes());
        }
    }
}
