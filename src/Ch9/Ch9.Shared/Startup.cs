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
			InitializeHttpClient(serviceProvider);
			InitializeBusinessServices(serviceProvider);
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

				client.BaseAddress = new Uri("https://ch9-app.azurewebsites.net/");

				return client;
			});
		}
	}
}
