using System.Collections.Generic;
using Ch9.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Windows.Input;
using Ch9.Domain;
using System.Threading.Tasks;

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

			DisplayShow = new RelayCommand<SourceFeed>(showFeed =>
			{
				App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(ShowPage), showFeed);
			});

			ReloadShowsList = new RelayCommand(LoadShowFeeds);

			LoadShowFeeds();
		}

		public ICommand ToAboutPage { get; }

		public ICommand ReloadShowsList { get; }

		public ICommand DisplayShow { get; set; }

		public TaskNotifier<IEnumerable<ShowItemViewModel>> _shows;
		public TaskNotifier<IEnumerable<ShowItemViewModel>> Shows
		{
			get => _shows;
			set => Set(() => Shows, ref _shows, value);
		}

		private ShowViewModel _show;
		public ShowViewModel Show
		{
			get => _show;
			set => Set(() => Show, ref _show, value);
		}

		public void OnNavigatedTo()
		{
			if (Show == null)
			{
				Show = new ShowViewModel();
			}
		}

		private void LoadShowFeeds()
		{
			async Task<IEnumerable<ShowItemViewModel>> GetShowFeeds()
			{
				var showFeeds = await Task.Run(async () => await App.ServiceProvider.GetInstance<IShowService>().GetShowFeeds());

				return showFeeds
					.OrderBy(s => s.Name)
					.Select(s => new ShowItemViewModel(this, s))
					.ToArray();
			}

			var result = new TaskNotifier<IEnumerable<ShowItemViewModel>>(GetShowFeeds());

			Shows = result;
		}
	}

	public class ShowItemViewModel
	{
		public ViewModelBase Parent { get; set; }

		public SourceFeed Show { get; set; }

		public ShowItemViewModel(ViewModelBase parent, SourceFeed show)
		{
			Parent = parent;
			Show = show;
		}
	}
}
