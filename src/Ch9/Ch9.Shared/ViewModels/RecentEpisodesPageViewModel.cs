using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using GalaSoft.MvvmLight;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class RecentEpisodesPageViewModel : ViewModelBase
	{
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

		public bool TryHandleBackRequested()
		{
			if (Show.SelectedEpisode != null)
			{
				if (Show.IsVideoFullWindow)
				{
					Show.IsVideoFullWindow = false;
				}
				else
				{
					Show.DismissSelectedEpisode.Execute(null);
				}

				return true;
			}

			return false;
		}
	}
}
