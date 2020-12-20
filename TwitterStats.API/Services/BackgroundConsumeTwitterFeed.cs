using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tweetinvi.Exceptions;

namespace TwitterStats.API.Services
{
    /*
     * There's lots of ways to go about this, but using aspnetcore the most logical seems to be a hosted background
     * service. This gets started by the startup invocation and just runs in the background.
     * The base type it inherits exposes additional methods to start and stop, but I don't think they're necessary in
     * this type of demonstration.
     *
     * If I were designing something to handle the entire twitter stream, I'd probably make some sort of standalone
     * worker service that fed data into a centralized queue, then another worker service to ingest data from the
     * queue into a central data repository (a document store, I imagine, for scalability and lack of need for
     * transactional data here). The API layer would then be greatly simplified since it would truly only be reading
     * data from that data store.
     */
    public class BackgroundConsumeTwitterFeed : BackgroundService
    {
        private readonly ICredentialProvider _credentialProvider;
        private readonly ILogger<BackgroundConsumeTwitterFeed> _logger;
        private readonly ITweetProcessor _tweetProcessor;

        public BackgroundConsumeTwitterFeed(ILogger<BackgroundConsumeTwitterFeed> logger,
            ITweetProcessor tweetProcessor, ICredentialProvider credentialProvider)
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