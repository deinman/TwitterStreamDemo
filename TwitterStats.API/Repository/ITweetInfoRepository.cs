using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterStats.API.Models;

namespace TwitterStats.API.Repository
{
    public interface ITweetInfoRepository
    {
        DateTime TimeStarted { get; }
        Task<long> GetCount();
        Task IncrementCount();
        Task IncrementCountWithUrl();
        Task IncrementCountWithUrlOfPhoto();
        Task AddAllEmojiCountDict(ICollection matches);
        Task AddSingleHashtagCountDict(string hashtag);
        Task AddSingleDomainCountDict(string domain);
        Task<TweetRate> GetTweetRate();
        Task<IEnumerable<KeyValuePair<string, int>>> GetTopEmoji(int count);
        Task<IEnumerable<KeyValuePair<string, int>>> GetTopHashtag(int count);
        Task<IEnumerable<KeyValuePair<string, int>>> GetTopDomain(int count);
        Task<int> GetPercentWithEmoji();
        Task<int> GetPercentWithUrl();
        Task<int> GetPercentWithUrlOfPhoto();
    }
}