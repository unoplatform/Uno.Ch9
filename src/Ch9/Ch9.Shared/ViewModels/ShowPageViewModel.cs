using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ch9.Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Ch9.ViewModels
{
    public class ShowPageViewModel : ViewModelBase
    {
        public ShowPageViewModel(SourceFeed sourceFeed)
        {
            SourceFeed = sourceFeed;

            Show = new ShowViewModel(sourceFeed);

            ToAboutPage = new RelayCommand(() =>
            {
                App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(AboutPage));
            });
        }

        public ICommand ToAboutPage { get; }

        public SourceFeed SourceFeed { get; }

        private ShowViewModel _show;
        public ShowViewModel Show
        {
            get => _show;
            set => Set(() => Show, ref _show, value);
        }
    }
}
