using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;
using Sibusten.Philomena.Client.Logging;

namespace Sibusten.Philomena.Client
{
    public static class PhilomenaClientRetryLogic
    {
        public static void UseDefaultHttpRetryLogic()
        {
            ILogger logger = Logger.Factory.CreateLogger(typeof(PhilomenaClientRetryLogic));

            // Use a jittered exponential backoff
            var delay = Backoff.DecorrelatedJitterBackoffV2
            (
                medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: 4
            );

            // Retry on transient http errors
            var defaultRetryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(delay, (result, timeout, attempt, context) =>
                {
                    logger.LogWarning(result.Exception, "Request #{Attempt} failed. Retrying in {Timeout}", attempt, timeout);
                });

            // Timeout requests
            var defaultTimeoutPolicy = Policy
                .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(2));

            // Wrap the default policies
            var defaultPolicy = Policy.WrapAsync(defaultRetryPolicy, defaultTimeoutPolicy);

            // Configure Flurl to use the backoff policy by overriding the HttpClientFactory
            FlurlHttp.Configure(settings =>
            {
                settings.HttpClientFactory = new PollyHttpClientFactory(defaultPolicy);
            });
        }

        /// <summary>
        /// A <see cref="DefaultHttpClientFactory"/> which wraps requests in a Polly policy
        /// </summary>
        public class PollyHttpClientFactory : DefaultHttpClientFactory
        {
            private AsyncPolicy<HttpResponseMessage> _retryPolicy;

            public PollyHttpClientFactory(AsyncPolicy<HttpResponseMessage> retryPolicy)
            {
                _retryPolicy = retryPolicy;
            }

            public override HttpMessageHandler CreateMessageHandler()
            {
                return new PolicyHandler(_retryPolicy)
                {
                    InnerHandler = base.CreateMessageHandler()
                };
            }

            public class PolicyHandler : DelegatingHandler
            {
                private AsyncPolicy<HttpResponseMessage> _retryPolicy;

                public PolicyHandler(AsyncPolicy<HttpResponseMessage> retryPolicy)
                {
                    _retryPolicy = retryPolicy;
                }

                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return _retryPolicy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
                }
            }
        }
    }
}
