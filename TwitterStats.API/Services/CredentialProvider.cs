﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterStats.API.Services
{
    public class CredentialProvider : ICredentialProvider
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CredentialProvider> _logger;

        public CredentialProvider(ILogger<CredentialProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        ///     Uses credentials in IConfiguration to build an authenticated Twitter client.
        /// </summary>
        /// <returns></returns>
        public async Task<TwitterClient> GetAuthenticatedTwitterClient()
        {
            _logger.LogDebug("Providing credentials");

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