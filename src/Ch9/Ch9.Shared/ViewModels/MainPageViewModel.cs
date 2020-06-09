using System.Collections.Generic;
using Ch9.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Windows.Input;
using Ch9.Domain;

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

            Shows = App.ServiceProvider.GetInstance<IShowService>()
                .GetShowFeeds()
                .OrderBy(s => s.Name)
                .ToArray();

            DisplayShow = new RelayCommand<SourceFeed>(showFeed  =>
            {
                App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(ShowPage), showFeed);
            });
        }

        public ICommand ToAboutPage { get; }

        public ICommand DisplayShow { get; set; }

        public IEnumerable<SourceFeed> Shows { get; set; }

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
    }
}
