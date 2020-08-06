using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Ch9.Views;

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
				var navigationService = new NavigationService()
				{
					CurrentFrame = Shell.Instance.Frame
				};

				navigationService.Configure(nameof(RecentEpisodesPage), typeof(RecentEpisodesPage));
				navigationService.Configure(nameof(ShowsPage), typeof(ShowsPage));
				navigationService.Configure(nameof(ShowPage), typeof(ShowPage));
				navigationService.Configure(nameof(AboutPage), typeof(AboutPage));

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
			App.ServiceProvider.GetInstance<IStackNavigationService>().NavigateTo(nameof(RecentEpisodesPage));
		}
	}
}
