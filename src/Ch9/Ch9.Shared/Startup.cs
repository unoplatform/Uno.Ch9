using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
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
			serviceProvider.Register<IShowService>(() => new ShowService(serviceProvider.GetInstance<HttpClient>()));
		}

		private void InitializeHttpClient(SimpleIoc serviceProvider)
		{
			serviceProvider.Register(() =>
			{
				var client = HttpUtility.CreateHttpClient();
				client.BaseAddress = new Uri("https://ch9-app.azurewebsites.net/api");
				return client;
			});
		}

		public void ExecuteInitialNavigation()
		{
			App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(MainPage));
		}
	}
}
