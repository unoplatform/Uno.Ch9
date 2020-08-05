using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Ch9
{
    internal static class HttpUtility
    {
	    internal static HttpClient HttpClient { get; } = CreateHttpClient();

        internal static HttpClient CreateHttpClient()
        {
#if __WASM__
            var httpClient = new HttpClient(new Uno.UI.Wasm.WasmHttpHandler());

			httpClient.DefaultRequestHeaders.Add("origin", "");
#else
			var httpClient = new HttpClient();
#endif
	        return httpClient;
		}

		internal static async Task<XmlReader> GetXmlReader(string url)
        {
#if __WASM__
			url = "https://ch9-app.azurewebsites.net/api/proxy?url=" + url;
#endif
			using (var response = await HttpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var stream = new MemoryStream(bytes);
                return XmlReader.Create(stream);
            }
        }
    }
}
