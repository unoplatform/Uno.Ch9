using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ShowViewModel : ObservableObject
    {
        private readonly SourceFeed _sourceFeed;

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

#if !__WASM__ && !__MACOS__
			ShareEpisode = new RelayCommand<EpisodeViewModel>(async episode =>
            {
                await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareTextRequest
                {
                    Uri = episode.Episode.EpisodeUri.ToString(),
                    Title = episode.Episode.Title
                });
            });
#endif

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
            set => SetProperty(ref _show, value);
        }

        private EpisodeViewModel[] _episodes;
        public EpisodeViewModel[] Episodes
        {
            get => _episodes;
            set => SetProperty(ref _episodes, value);
        }

        private EpisodeViewModel _selectedEpisode;
        public EpisodeViewModel SelectedEpisode
        {
            get => _selectedEpisode;
            set => SetProperty(ref _selectedEpisode, value);
        }

        private bool _isVideoFullWindow;
        public bool IsVideoFullWindow
        {
            get => _isVideoFullWindow;

            set
            {
				SetProperty(ref _isVideoFullWindow, value);

                App.Instance.OnFullscreenChanged(value);
            }
        }

        private void LoadShow()
        {
            async Task<Show> GetShow()
            {
                var show = await Task.Run(() => Ioc.Default.GetService<IShowService>().GetShow(_sourceFeed));

                Episodes = show.Episodes.Select(p => new EpisodeViewModel(this, p)).ToArray();

                return show;
            }

            Show = new TaskNotifier<Show>(GetShow());
        }
    }
}
