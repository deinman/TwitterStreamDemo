using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models.V2;
using TwitterStats.API.Repository;

namespace TwitterStats.API.Services
{
    public class TweetProcessor : ITweetProcessor
    {
        private readonly ITweetInfoRepository _tweetInfoRepository;
        public TweetProcessor(ITweetInfoRepository tweetInfoRepository)
        {
            _tweetInfoRepository = tweetInfoRepository;
        }

        /// <summary>
        /// This is the main entrypoint for collecting info on tweets.
        /// </summary>
        /// <param name="tweet"></param>
        public void ProcessTweet(TweetV2 tweet)
        {
            _tweetInfoRepository.IncrementCount();
            
            ProcessEmoji(tweet.Text);
            ProcessHashtags(tweet.Text);

            // todo: rework hashtags
            //var temp = tweet.Entities.Hashtags;
            
            if (tweet.Entities?.Urls != null && (bool) tweet.Entities?.Urls.Any())
            {
                ProcessUrls(tweet.Entities.Urls);
            }
        }

        private void ProcessEmoji(string tweet)
        {
            var matches = Extensions.GetEmoji(tweet);
            if (!matches.Any())
            {
                return;
            }
            
            _tweetInfoRepository.AddToEmojiCountDict(matches);
        }

        private void ProcessHashtags(string tweet)
        {
            var matches = Extensions.GetHashtags(tweet);
            if (!matches.Any())
            {
                return;
            }
            
            _tweetInfoRepository.AddToHashtagCountDict(matches);
        }

        private void ProcessUrls(IReadOnlyCollection<UrlV2> entitiesUrls)
        {
            if (entitiesUrls == null || !entitiesUrls.Any())
            {
                return;
            }

            foreach (var url in entitiesUrls)
            {
                var domain = Extensions.GetDomain(url.DisplayUrl).ToString();

                if (string.IsNullOrEmpty(domain))
                {
                    throw new Exception($"Unexpected null: {nameof(domain)}");
                }
                
                _tweetInfoRepository.AddToDomainCountDict(domain);
                
                if (IsImageUrl(domain))
                {
                    _tweetInfoRepository.IncrementCountWithUrlOfPhoto();
                }
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