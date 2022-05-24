using System;
using FunctionAppPolly;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Logging;

[assembly: WebJobsStartup(typeof(Startup))]

namespace FunctionAppPolly
{
    class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddHttpClient("HttpClient").AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {

            return HttpPolicyExtensions
              // Handle HttpRequestExceptions, 408 and 5xx status codes
              .HandleTransientHttpError()
              //.HandleTransientHttpError()
              // Handle 404 not found
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              // Handle 401 Unauthorized
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
              // What to do if any of the above erros occur:
              // Retry 3 times, each time wait 5 seconds before retrying.
              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5)); // Math.Pow(2, retryAttempt)

        }

    }

}




