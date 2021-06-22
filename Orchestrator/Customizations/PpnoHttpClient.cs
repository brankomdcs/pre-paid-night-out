using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orchestrator.Customizations
{
    public class PpnoHttpClient
    {
        public HttpClient BaseClient { get; }

        public PpnoHttpClient(HttpClient httpClient)
        {
            BaseClient = httpClient;
        }

        public async Task<IActionResult> GetAsync(string getUrl)
        {
            using (HttpResponseMessage response = await BaseClient.GetAsync(getUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // Making this flow to throw exception on purpose:
                    throw new System.Exception($"Communication failed with the service on url: {getUrl} \n Status code response: {response.StatusCode}");
                }

                return new ContentResult()
                {
                    StatusCode = (int)response.StatusCode,
                    Content = await response.Content.ReadAsStringAsync()
                };
            }
        }

        public async Task<IActionResult> PostAsync(string postUrl, StringContent postContent = null) 
        {
            using (HttpResponseMessage response = await BaseClient.PostAsync(postUrl, postContent))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // Making this flow to throw exception on purpose:
                    throw new System.Exception($"Communication failed with the service on url: {postUrl} \n Status code response: {response.StatusCode}");
                }

                return new ContentResult()
                {
                    StatusCode = (int)response.StatusCode,
                    Content = await response.Content.ReadAsStringAsync()
                };
            }
        }

        public async Task<T> GetDeserializedResponse<T>(string getUrl)
        {
            using (HttpResponseMessage response = await BaseClient.GetAsync(getUrl))
            {
                if (!response.IsSuccessStatusCode) {
                    // Making this flow to throw exception on purpose:
                    throw new System.Exception($"Communication failed with the service on url: {getUrl} \n Status code response: {response.StatusCode}");
                }

                var contentJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(contentJson);
            }
        }
    }
}
