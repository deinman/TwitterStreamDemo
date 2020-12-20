using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Tweetinvi.Models.V2;
using TwitterStats.API.Repository;

namespace TwitterStats.API.Services
{
    public class TweetProcessor : ITweetProcessor
    {
        private readonly ILogger<TweetProcessor> _logger;
        private readonly ITweetInfoRepository _tweetInfoRepository;

        public TweetProcessor(ITweetInfoRepository tweetInfoRepository, ILogger<TweetProcessor> logger)
        {
            _tweetInfoRepository = tweetInfoRepository;
            _logger = logger;
        }

        /// <summary>
        ///     This is the main entrypoint for collecting info on tweets.
        /// </summary>
        /// <param name="tweet"></param>
        public void ProcessTweet(TweetV2 tweet)
        {
            _tweetInfoRepository.IncrementCount();

            ProcessEmoji(tweet.Text);

            if (tweet.Entities?.Hashtags != null && (bool) tweet.Entities?.Hashtags.Any())
                ProcessHashtags(tweet.Entities.Hashtags);

            if (tweet.Entities?.Urls != null && (bool) tweet.Entities?.Urls.Any()) ProcessUrls(tweet.Entities.Urls);
        }

        private void ProcessEmoji(string tweet)
        {
            var matches = Extensions.GetEmoji(tweet);
            if (!matches.Any()) return;

            _tweetInfoRepository.AddAllEmojiCountDict(matches);
        }

        private void ProcessHashtags(IReadOnlyCollection<HashtagV2> entitiesHashtags)
        {
            if (entitiesHashtags == null || entitiesHashtags.Count == 0) return;

            foreach (var hashtag in entitiesHashtags) _tweetInfoRepository.AddSingleHashtagCountDict(hashtag.Tag);
        }

        private void ProcessUrls(IReadOnlyCollection<UrlV2> entitiesUrls)
        {
            if (entitiesUrls == null || !entitiesUrls.Any()) return;

            _tweetInfoRepository.IncrementCountWithUrl();

            foreach (var url in entitiesUrls)
            {
                var domain = Extensions.GetDomain(url.DisplayUrl).ToString();

                if (string.IsNullOrEmpty(domain))
                    _logger.LogError("Unexpected null domain. Url: {url}", url.DisplayUrl);

                _tweetInfoRepository.AddSingleDomainCountDict(domain);

                if (IsImageUrl(domain)) _tweetInfoRepository.IncrementCountWithUrlOfPhoto();
            }
        }

        private bool IsImageUrl(string url)
        {
            // This could be more dynamic, maybe populated by a DB table?
            var imgUrlList = new List<string>
            {
                "pic.twitter.com",
                "instagram.com",
                "pic.instagram.com"
            };

            return imgUrlList.Contains(url);
        }
    }
}