using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using Ch9.Client;
using System.Net.Http;

namespace Ch9
{
	public class Startup
	{
		public void Initialize(SimpleIoc serviceProvider)
		{
			InitializeNavigationService(serviceProvider);
			InitializeHttpClient(serviceProvider);
			InitializeBusinessServices(serviceProvider);
		}

		private void InitializeNavigationService(SimpleIoc serviceProvider)
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

		private void InitializeBusinessServices(SimpleIoc serviceProvider)
		{
			serviceProvider.Register<IShowService>(() => new ShowService());
		}

		private void InitializeHttpClient(SimpleIoc serviceProvider)
		{
			serviceProvider.Register(() => new HttpClient(new HttpClientHandler(), false)
			{
				BaseAddress = new Uri(ClientConstants.BaseApiUrl)
			});
		}
	}
}
