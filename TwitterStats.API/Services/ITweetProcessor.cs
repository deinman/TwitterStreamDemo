using Tweetinvi.Models.V2;

namespace TwitterStats.API.Services
{
    public interface ITweetProcessor
    {
        /// <summary>
        /// This is the main entrypoint for collecting info on tweets.
        /// </summary>
        /// <param name="tweet"></param>
        void ProcessTweet(TweetV2 tweet);
    }
}