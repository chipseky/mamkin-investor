using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class RetryPoliceHelper
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILoggerFactory loggerFactory)
    {
        var random = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.Conflict)
            .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                  + TimeSpan.FromMilliseconds(random.Next(0, 100)), onRetry: (result, delay, retryAttempt, _) =>
            {
                var logger = loggerFactory.CreateLogger("RetryHttpRequestPolicy");
                logger.LogWarning($"Delaying for {delay} before {retryAttempt}-th retry request because {result.Exception?.Message ?? "StatusCode: " + result.Result.StatusCode}");
            });
    }
}