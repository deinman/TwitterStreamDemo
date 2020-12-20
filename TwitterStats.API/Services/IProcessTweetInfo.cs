using System.Collections;
using System.Collections.Generic;
using Tweetinvi.Models.V2;
using TwitterStats.API.Models;

namespace TwitterStats.API.Services
{
    public interface IProcessTweetInfo
    {
        public void ProcessTweet(TweetV2 tweet);
        public long GetCount();
        public int GetPercentWithEmoji();
        public int GetPercentWithUrl();
        public int GetPercentWithUrlOfPhoto();
        public TweetRate GetTweetRate();
        public IEnumerable<KeyValuePair<string, int>> GetTopEmoji(int count);
        public IEnumerable<KeyValuePair<string, int>> GetTopHashtag(int count);
        public IEnumerable<KeyValuePair<string, int>> GetTopDomain(int count);
    }
}