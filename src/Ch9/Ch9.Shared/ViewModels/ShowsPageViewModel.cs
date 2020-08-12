using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using Ch9.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ShowsPageViewModel : ObservableObject
	{
		public ShowsPageViewModel()
		{
			DisplayShow = new RelayCommand<SourceFeed>(showFeed =>
			{
				Shell.Instance.NavigateTo(typeof(ShowPage), showFeed);
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
			set => SetProperty(ref _shows, value);
		}

		private void LoadShowFeeds()
		{
			async Task<IEnumerable<ShowItemViewModel>> GetShowFeeds()
			{
				var showFeeds = await Task.Run(() => Ioc.Default.GetService<IShowService>().GetShowFeeds());

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
		public ObservableObject Parent { get; set; }

		public SourceFeed Show { get; set; }

		public ShowItemViewModel(ObservableObject parent, SourceFeed show)
		{
			Parent = parent;
			Show = show;
		}
	}
}
