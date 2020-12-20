using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tweetinvi.Models.V2;
using TwitterStats.API.Services;
using Xunit;

namespace TwitterStats.UnitTest
{
    public class ProcessInfoTest
    {
        private ProcessTweetInfo BuildProcessTweetInfo()
        {
            return new();
        }

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
        public void Increment_count_success()
        {
            // Arrange
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("Test");

            // Act
            info.ProcessTweet(tweet);
            var resultCount = info.GetCount();
            
            // Assert
            Assert.Equal(1, resultCount);
        }

        [Fact]
        public void Get_rate_success()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("Test");
            
            info.ProcessTweet(tweet);
            
            Thread.Sleep(1000);

            var rate = info.GetTweetRate();
            
            Assert.Equal(1, rate.TweetsPerSecond);
        }

        [Fact]
        public void No_emoji_produces_no_emoji()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("Test");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(0, info.GetPercentWithEmoji());
        }

        [Fact]
        public void Emoji_produces_emoji()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("❤");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(100, info.GetPercentWithEmoji());
        }

        [Fact]
        public void Calculate_emoji_rate_success()
        {
            var info = BuildProcessTweetInfo();
            var t = BuildTestTweet("Test");

            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }

            var topEmoji = info.GetTopEmoji(1).First().Key;
            
            Assert.Equal(17, info.GetPercentWithEmoji());
            Assert.Equal("❤", topEmoji);
        }
        
        [Fact]
        public void Hashtag_is_counted()
        {
            var info = BuildProcessTweetInfo();

            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }

            var topHashtag = info.GetTopHashtag(1).First().Key;
            
            Assert.Equal("#Three", topHashtag);
        }

        [Fact]
        public void Multiple_hashtags_in_tweet()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("#Blessed #Mission");

            info.ProcessTweet(tweet);

            var result = info.GetTopHashtag(2);
            
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Identify_tweets_with_url()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("Test", "https://www.google.com");
            
            info.ProcessTweet(tweet);

            var result = info.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }

        [Fact]
        public void Identify_tweets_with_image_url()
        {
            var info = BuildProcessTweetInfo();
            var tweet = BuildTestTweet("Test", "http://pic.twitter.com/ABCDEFG");
            
            info.ProcessTweet(tweet);

            var result = info.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }

        [Fact]
        public void Calculate_percentage_of_urls()
        {
            var info = BuildProcessTweetInfo();

            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }

            var percentWithUrl = info.GetPercentWithUrl();
            var percentWithUrlImg = info.GetPercentWithUrlOfPhoto();
            
            Assert.Equal(50, percentWithUrl);
            Assert.Equal(17, percentWithUrlImg);
        }

        [Fact]
        public void Urls_Count_Success()
        {
            var info = BuildProcessTweetInfo();

            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }

            var result = info.GetTopDomain(2).First();
            
            Assert.Equal("www.google.com", result.Key);
            Assert.Equal(2, result.Value);
        }
    }
}