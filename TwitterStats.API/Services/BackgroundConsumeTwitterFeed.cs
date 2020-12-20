using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tweetinvi.Exceptions;

namespace TwitterStats.API.Services
{
    public class BackgroundConsumeTwitterFeed : BackgroundService
    {
        private readonly ILogger<BackgroundConsumeTwitterFeed> _logger;
        private readonly IProcessTweetInfo _tweetProcessor;
        private readonly ICredentialProvider _credentialProvider;

        public BackgroundConsumeTwitterFeed(ILogger<BackgroundConsumeTwitterFeed> logger, IProcessTweetInfo tweetProcessor, ICredentialProvider credentialProvider)
        {
            _logger = logger;
            _tweetProcessor = tweetProcessor;
            _credentialProvider = credentialProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("{serviceName} is starting", nameof(BackgroundConsumeTwitterFeed));

            stoppingToken.Register(() =>
                _logger.LogDebug("{serviceName} background task is stopping.", nameof(BackgroundConsumeTwitterFeed)));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("{serviceName} task doing background work.", nameof(BackgroundConsumeTwitterFeed));

                var client = await _credentialProvider.GetAuthenticatedTwitterClient();
                var sampleStream = client.StreamsV2.CreateSampleStream();

                sampleStream.TweetReceived += (_, eventArgs) =>
                {
                    try
                    {
                        _tweetProcessor.ProcessTweet(eventArgs.Tweet);
                    }
                    catch (TwitterAuthException e)
                    {
                        _logger.LogError(e, "Auth exception, check credentials");
                    }
                    catch (TwitterException e)
                    {
                        _logger.LogError(e, "Error from twitter");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error processing tweet");
                    }
                };

                await sampleStream.StartAsync();
            }

            _logger.LogDebug("{serviceName} background task is stopping.", nameof(BackgroundConsumeTwitterFeed));

            await Task.CompletedTask;
        }
    }
}