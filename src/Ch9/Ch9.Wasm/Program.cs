using System;
using System.Net.Http;
using System.Reflection;
using Windows.UI.Xaml;

namespace Ch9.Wasm
{
	public class Program
	{
		private static App _app;

		static int Main(string[] args)
		{
			SetDefaultHttpHandler();

			Windows.UI.Xaml.Application.Start(_ => _app = new App());

			return 0;
		}

		private static void SetDefaultHttpHandler()
		{
			var type = Type
				.GetType("System.Net.Http.HttpClient, System.Net.Http");

			var methods = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

			foreach (var m in methods)
			{
				Console.WriteLine(m.Name);
			}

			var httpMessageHandler = type
				.GetField(
					"GetHttpMessageHandler",
					BindingFlags.Static | BindingFlags.NonPublic
				);

			httpMessageHandler?.SetValue(
				null,
				(Func<HttpMessageHandler>)(() => new Uno.UI.Wasm.WasmHttpHandler())
			);
		}
	}
}
