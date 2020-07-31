using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Ch9.Client
{
    internal static class HttpUtility
    {
	    internal static HttpClient HttpClient { get; } = CreateHttpClient();

        internal static HttpClient CreateHttpClient()
        {
#if __WASM__
            return new HttpClient(new Uno.UI.Wasm.WasmHttpHandler());
#else
            return new HttpClient();
#endif

		}

		internal static async Task<XmlReader> GetXmlReader(string url)
        {
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
