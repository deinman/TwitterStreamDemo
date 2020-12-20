using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterStats.API.Services
{
    public class CredentialProvider : ICredentialProvider
    {
        private readonly ILogger<CredentialProvider> _logger;
        private readonly IConfiguration _configuration;

        public CredentialProvider(ILogger<CredentialProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<TwitterClient> GetAuthenticatedTwitterClient()
        {
            var tempClient = new TwitterClient(new ConsumerOnlyCredentials(
                _configuration["CONSUMER_KEY"],
                _configuration["CONSUMER_SECRET"]));

            var bearer = await tempClient.Auth.CreateBearerTokenAsync();
            var appCreds = new ConsumerOnlyCredentials(new ConsumerOnlyCredentials(
                _configuration["CONSUMER_KEY"],
                _configuration["CONSUMER_SECRET"]))
            {
                BearerToken = bearer
            };
            
            return new TwitterClient(appCreds);
        }
    }
}