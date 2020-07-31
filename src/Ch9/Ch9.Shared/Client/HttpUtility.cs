using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Uno.UI.Wasm;

namespace Ch9.Client
{
    internal static class HttpUtility
    {
        private static HttpClient _httpClient;

        static HttpUtility()
        {
#if __WASM__
            _httpClient = new HttpClient(new WasmHttpHandler());
#else
            _httpClient = new HttpClient();
#endif

        }

        internal static async Task<XmlReader> GetXmlReader(string url)
        {
            using (var response = await _httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var stream = new MemoryStream(bytes);
                return XmlReader.Create(stream);
            }
        }
    }
}
