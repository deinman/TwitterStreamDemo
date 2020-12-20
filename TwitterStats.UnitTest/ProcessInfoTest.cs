using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using TwitterStats.API.Repository;
using TwitterStats.API.Services;
using Xunit;

namespace TwitterStats.UnitTest
{
    public class ProcessInfoTest
    {
        private IEnumerable<TweetV2> BuildListOfTweets()
        {
            return new List<TweetV2>
            {
                BuildTestTweet("Test"),
                BuildTestTweet("On❤"),
                BuildTestTweet("Test","pic.instagram.com/"),
                BuildTestTweet("#Three"),
                BuildTestTweet("Five","www.google.com/test"),
                BuildTestTweet("Six", "www.google.com")
            };
        }
        
        private TweetV2 BuildTestTweet(string tweetText)
        {
            return new()
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Urls = new UrlV2[]
                    {
                        
                    }
                }
            };
        }

        private TweetV2 BuildTestTweet(string tweetText, string url)
        {
            
            return new()
            {
                Text = tweetText,
                Entities = new TweetEntitiesV2()
                {
                    Urls = new UrlV2[]
                    {
                        new ()
                        {
                            DisplayUrl = url
                        }
                    }
                }
            };
        }
        
        [Fact]
        public async void Increment_count_success()
        {
            // Arrange
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("Test");

            // Act
            info.ProcessTweet(tweet);
            var resultCount = await repo.GetCount();
            
            // Assert
            Assert.Equal(1, resultCount);
        }

        [Fact]
        public async void Get_rate_success()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("Test");
            
            info.ProcessTweet(tweet);
            
            Thread.Sleep(1000);
        
            var rate = await repo.GetTweetRate();
            
            Assert.Equal(1, rate.TweetsPerSecond);
        }

        [Fact]
        public async void No_emoji_produces_no_emoji()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("Test");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(0, await repo.GetPercentWithEmoji());
        }
        
        [Fact]
        public async void Emoji_produces_emoji()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("❤");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(100, await repo.GetPercentWithEmoji());
        }
        
        [Fact]
        public async void Calculate_emoji_rate_success()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
        
            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }
        
            var topEmoji = await repo.GetTopEmoji(1);
            
            Assert.Equal(17, await repo.GetPercentWithEmoji());
            Assert.Equal("❤", topEmoji.First().Key);
        }
        
        [Fact]
        public async void Hashtag_is_counted()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
        
            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }
        
            var topHashtag = await repo.GetTopHashtag(1);
            
            Assert.Equal("#Three", topHashtag.First().Key);
        }
        
        [Fact]
        public async void Multiple_hashtags_in_tweet()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("#Blessed #Mission");
        
            info.ProcessTweet(tweet);
        
            var result = await repo.GetTopHashtag(2);
            
            Assert.Equal(2, result.Count());
        }
        
        [Fact]
        public async void Identify_tweets_with_url()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("Test", "https://www.google.com");
            
            info.ProcessTweet(tweet);
        
            var result = await repo.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }
        
        [Fact]
        public async void Identify_tweets_with_image_url()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
            var tweet = BuildTestTweet("Test", "http://pic.twitter.com/ABCDEFG");
            
            info.ProcessTweet(tweet);
        
            var result = await repo.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }
        
        [Fact]
        public async void Calculate_percentage_of_urls()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
        
            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }
        
            var percentWithUrl = await repo.GetPercentWithUrl();
            var percentWithUrlImg = await repo.GetPercentWithUrlOfPhoto();
            
            Assert.Equal(50, percentWithUrl);
            Assert.Equal(17, percentWithUrlImg);
        }
        
        [Fact]
        public async void Urls_Count_Success()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo);
        
            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }
        
            var result = await repo.GetTopDomain(2);
            
            Assert.Equal("www.google.com", result.First().Key);
            Assert.Equal(2, result.First().Value);
        }
    }
}