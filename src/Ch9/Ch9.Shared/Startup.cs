using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Ch9
{
	public class Startup
	{
		public void Initialize(Ioc ioc)
		{
			ioc.ConfigureServices(services =>
			{
				InitializeHttpClient(services);
				InitializeBusinessServices(services);
			});
		}

		private void InitializeBusinessServices(IServiceCollection serviceProvider)
		{
			serviceProvider.AddSingleton<IShowService, ShowService>();
		}

		private void InitializeHttpClient(IServiceCollection serviceProvider)
		{
			serviceProvider.AddTransient(s =>
			{
				var client = HttpUtility.CreateHttpClient();

				client.BaseAddress = new Uri("https://ch9-app.azurewebsites.net/");

				return client;
			});
		}
	}
}
