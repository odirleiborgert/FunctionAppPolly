using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace FunctionAppPolly
{
    public class MyPolly
    {

        private readonly IHttpClientFactory _clientFactory;


        public MyPolly(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        [FunctionName("MyPolly")]
        public async Task Run([TimerTrigger("*/20 * * * * *")]TimerInfo myTimer, ILogger log)
        {

            Guid stringGuid = Guid.NewGuid();

            log.LogError("-----------------------------------------------------------------------------------------");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var httpClient = _clientFactory.CreateClient("HttpClient");

            httpClient.BaseAddress = new Uri("https://webhook.site/");
            httpClient.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await httpClient.GetAsync("c8d9b8c0-46a1-43f9-a4b6-805390fa6ca5?id=" + stringGuid);

            if (response != null && response.IsSuccessStatusCode)
            {

                string stringContent = await response.Content.ReadAsStringAsync();

                ResultJson content = JsonSerializer.Deserialize<ResultJson>(stringContent);

                log.LogInformation("Status request " + response.StatusCode + " - Message: " + content.message);
            }

            log.LogError("-----------------------------------------------------------------------------------------");




        }
    }


    class ResultJson
    {
        public string message { get; set; }
    }
}
