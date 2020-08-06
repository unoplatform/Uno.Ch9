using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using Ch9.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ShowsPageViewModel : ViewModelBase
	{
		public ShowsPageViewModel()
		{
			DisplayShow = new RelayCommand<SourceFeed>(showFeed =>
			{
				App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(ShowPage), showFeed);
			});

			ReloadShowsList = new RelayCommand(LoadShowFeeds);

			LoadShowFeeds();
		}

		public ICommand ReloadShowsList { get; }

		public ICommand DisplayShow { get; set; }

		private TaskNotifier<IEnumerable<ShowItemViewModel>> _shows;
		public TaskNotifier<IEnumerable<ShowItemViewModel>> Shows
		{
			get => _shows;
			set => Set(() => Shows, ref _shows, value);
		}

		private void LoadShowFeeds()
		{
			async Task<IEnumerable<ShowItemViewModel>> GetShowFeeds()
			{
				var showFeeds = await Task.Run(() => App.ServiceProvider.GetInstance<IShowService>().GetShowFeeds());

				return showFeeds
					.OrderBy(s => s.Name)
					.Select(s => new ShowItemViewModel(this, s))
					.ToArray();
			}

			Shows = new TaskNotifier<IEnumerable<ShowItemViewModel>>(GetShowFeeds());
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
