using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStats.API.Models;

namespace TwitterStats.API.Repository
{
    /*
     * If I were storing this in a DB I'd need logic to sync it up. As is, the mechanism of just having it run as a
     * singleton seems to work great. I went ahead and made accessor methods async because if we were working with
     * external storage like a DB we would need to account for the async read/write operations.
     * 
     * You'll see a lot of calls to await Task.CompletedTask
     *      - these are simply in place to silence the unused-async warnings.
     */
    public class TweetInfoRepository : ITweetInfoRepository
    {
        public TweetInfoRepository()
        {
            TimeStarted = DateTime.Now;
        }
        
        public DateTime TimeStarted { get; }
        private long Count { get; set; }
        private long CountWithEmoji { get; set; }
        private long CountWithUrl { get; set; }
        private long CountWithUrlOfPhoto { get; set; }
        private Dictionary<string, int> EmojiCountDict { get; } = new();
        private Dictionary<string, int> HashtagCountDict { get; } = new();
        private Dictionary<string, int> DomainCountDict { get; } = new();

        public async Task<long> GetCount()
        {
            await Task.CompletedTask;
            return Count;
        }

        public async Task IncrementCount()
        {
            await Task.CompletedTask;
            Count++;
        }

        public async Task IncrementCountWithUrl()
        {
            await Task.CompletedTask;
            CountWithUrl++;
        }
        
        public async Task IncrementCountWithUrlOfPhoto()
        {
            await Task.CompletedTask;
            CountWithUrlOfPhoto++;
        }

        public async Task AddAllEmojiCountDict(ICollection matches)
        {
            await Task.CompletedTask;
            // Since we're adding all here, we can bind the CountWithEmoji to the same method.
            CountWithEmoji += matches.Count;
            Extensions.AddManyToCountDict(matches, EmojiCountDict);
        }

        public async Task AddSingleHashtagCountDict(string hashtag)
        {
            await Task.CompletedTask;
            Extensions.AddSingleToCountDict(hashtag, HashtagCountDict);
        }

        public async Task AddSingleDomainCountDict(string domain)
        {
            await Task.CompletedTask;
            Extensions.AddSingleToCountDict(domain, DomainCountDict);
        }
        
        public async Task<TweetRate> GetTweetRate()
        {
            await Task.CompletedTask;
            var runningSeconds = (DateTime.Now - TimeStarted).TotalSeconds;
            var ret = new TweetRate
            {
                TweetsPerSecond = (int)Math.Ceiling(Count / runningSeconds)
            };
            return ret;
        }

        public async Task<IEnumerable<KeyValuePair<string, int>>> GetTopEmoji(int count)
        {
            await Task.CompletedTask;
            return EmojiCountDict.OrderByDescending(x => x.Value).Take(count);
        }
        
        public async Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtag(int count)
        {
            await Task.CompletedTask;
            return HashtagCountDict.OrderByDescending(x => x.Value).Take(count);
        }
        public async Task<IEnumerable<KeyValuePair<string, int>>> GetTopDomain(int count)
        {
            await Task.CompletedTask;
            return DomainCountDict.OrderByDescending(x => x.Value).Take(count);
        }

        public async Task<int> GetPercentWithEmoji()
        {
            await Task.CompletedTask;
            return GetPercentFromCount(CountWithEmoji, Count);
        }

        public async Task<int> GetPercentWithUrl()
        {
            await Task.CompletedTask;
            return GetPercentFromCount(CountWithUrl, Count);
        }

        public async Task<int> GetPercentWithUrlOfPhoto()
        {
            await Task.CompletedTask;
            return GetPercentFromCount(CountWithUrlOfPhoto, Count);
        }

        private static int GetPercentFromCount(long num, long total)
        {
            return (int) Math.Round((double) (100 * num) / total);
        }
    }
}