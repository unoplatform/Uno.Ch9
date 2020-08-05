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
#else
			var httpClient = new HttpClient();
#endif

			// TODO Find proper origin value
			httpClient.DefaultRequestHeaders.Add("origin", "");

			return httpClient;
		}

		internal static async Task<XmlReader> GetXmlReader(string url)
        {
#if __WASM__
			url = "https://cors-anywhere.herokuapp.com/" + url;
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
