using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9
{
	public class Startup
	{
		public void Initialize(ISimpleIoc serviceProvider)
		{
			InitializeNavigationService(serviceProvider);
            InitializeBusinessServices(serviceProvider);
		}

		private void InitializeNavigationService(ISimpleIoc serviceProvider)
		{
			serviceProvider.Register<IStackNavigationService>(() =>
			{
				var navigationService = new NavigationService();

				navigationService.Configure(nameof(MainPage), typeof(MainPage));
				navigationService.Configure(nameof(AboutPage), typeof(AboutPage));
                navigationService.Configure(nameof(ShowPage), typeof(ShowPage));

				return new StackNavigationService(navigationService);
			});
		}

		private void InitializeBusinessServices(ISimpleIoc serviceProvider)
		{
            serviceProvider.Register<IShowService>(() => new ShowService());
		}
	}
}
