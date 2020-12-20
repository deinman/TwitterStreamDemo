using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models.V2;
using TwitterStats.API.Models;

namespace TwitterStats.API.Services
{
    public class ProcessTweetInfo : IProcessTweetInfo
    {
        public ProcessTweetInfo()
        {
            TimeStarted = DateTime.Now;
        }

        public void ProcessTweet(TweetV2 tweet)
        {
            Count++;
            
            ProcessEmoji(tweet.Text);
            ProcessHashtags(tweet.Text);

            // todo: rework hashtags
            //var temp = tweet.Entities.Hashtags;
            
            if (tweet.Entities?.Urls != null && (bool) tweet.Entities?.Urls.Any())
            {
                ProcessUrls(tweet.Entities.Urls);
            }
        }

        public long GetCount()
        {
            return Count;
        }

        private void ProcessEmoji(string tweet)
        {
            var matches = Extensions.GetEmoji(tweet);
            if (!matches.Any())
            {
                return;
            }

            CountWithEmoji++;
            Extensions.AddManyToCountDict(matches, EmojiCountDict);
        }

        private void ProcessHashtags(string tweet)
        {
            var matches = Extensions.GetHashtags(tweet);
            if (!matches.Any())
            {
                return;
            }
            
            Extensions.AddManyToCountDict(matches, HashtagCountDict);
        }

        private void ProcessUrls(IReadOnlyCollection<UrlV2> entitiesUrls)
        {
            if (entitiesUrls == null || !entitiesUrls.Any())
            {
                return;
            }

            CountWithUrl += entitiesUrls.Count;

            foreach (var url in entitiesUrls)
            {
                var domain = Extensions.GetDomain(url.DisplayUrl).ToString();

                if (string.IsNullOrEmpty(domain))
                {
                    throw new Exception($"Unexpected null: {nameof(domain)}");
                }
                
                Extensions.AddSingleToCountDict(domain, DomainCountDict);

                if (IsImageUrl(domain))
                {
                    CountWithUrlOfPhoto++;
                }
            }
        }

        private bool IsImageUrl(string url)
        {
            return url.Contains("pic.twitter.com") || url.Contains("instagram.com");
        }
        
        

        public TweetRate GetTweetRate()
        {
            var runningSeconds = (DateTime.Now - TimeStarted).TotalSeconds;
            var ret = new TweetRate()
            {
                TweetsPerSecond = (int)Math.Ceiling(Count / runningSeconds)
            };
            return ret;
        }

        public IEnumerable<KeyValuePair<string, int>> GetTopEmoji(int count)
        {
            return EmojiCountDict.OrderByDescending(x => x.Value).Take(count);
        }
        
        public IEnumerable<KeyValuePair<string, int>> GetTopHashtag(int count)
        {
            return HashtagCountDict.OrderByDescending(x => x.Value).Take(count);
        }
        
        public IEnumerable<KeyValuePair<string, int>> GetTopDomain(int count)
        {
            return DomainCountDict.OrderByDescending(x => x.Value).Take(count);
        }

        public int GetPercentWithEmoji()
        {
            return GetPercentFromCount(CountWithEmoji, Count);
        }

        public int GetPercentWithUrl()
        {
            return GetPercentFromCount(CountWithUrl, Count);
        }

        public int GetPercentWithUrlOfPhoto()
        {
            return GetPercentFromCount(CountWithUrlOfPhoto, Count);
        }

        private int GetPercentFromCount(long num, long total)
        {
            return (int) Math.Round((double) (100 * num) / total);
        }

        private long Count { get; set; }
        private long CountWithEmoji { get; set; }
        private long CountWithUrl { get; set; }
        private long CountWithUrlOfPhoto { get; set; }
        private DateTime TimeStarted { get; }
        private Dictionary<string, int> EmojiCountDict { get; } = new();
        private Dictionary<string, int> HashtagCountDict { get; } = new();
        private Dictionary<string, int> DomainCountDict { get; } = new();
    }
}