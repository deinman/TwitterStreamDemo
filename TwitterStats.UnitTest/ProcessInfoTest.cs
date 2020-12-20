using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Tweetinvi.Models.V2;
using TwitterStats.API.Repository;
using TwitterStats.API.Services;
using Xunit;

namespace TwitterStats.UnitTest
{
    /*
     * So this encompasses a bulk of the testing. I should probably test the TweetInfoRepository on its own, but every
     * mechanism to work with it is through the context of the TweetProcessor, so I decided to save some time/effort.
     * The naming of the methods could probably use more work/consistency, but it's good-enough
     * 
     */
    public class ProcessInfoTest
    {
        private readonly Mock<ILogger<TweetProcessor>> _mockLogger;
        public ProcessInfoTest()
        {
            _mockLogger = new Mock<ILogger<TweetProcessor>>();
        }

        private IEnumerable<TweetV2> BuildListOfTweets()
        {
            return new List<TweetV2>
            {
                BuildTestTweet("Test"),
                BuildTestTweet("On❤"),
                BuildTestTweet("Test","pic.instagram.com/"),
                BuildTestTweet("#Three three", "null.com", "#Three"),
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
                        
                    },
                    Hashtags = new HashtagV2[]
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
                    },
                    Hashtags = new HashtagV2[]
                    {
                        
                    }
                }
            };
        }
        
        private TweetV2 BuildTestTweet(string tweetText, string url, string hashtag)
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
                    },
                    Hashtags = new HashtagV2[]
                    {
                        new HashtagV2()
                        {
                            Hashtag = hashtag
                        }
                    }
                }
            };
        }
        
        private TweetV2 BuildTestTweet(string tweetText, string url, string[] hashtags)
        {
            var t = new TweetV2()
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
                    },
                    Hashtags = new HashtagV2[]
                    {
                        new HashtagV2()
                        {
                            Hashtag = hashtags[0]
                        },
                        new HashtagV2()
                        {
                            Hashtag = hashtags[1]
                        }
                    }
                }
            };

            return t;
        }
        
        [Fact]
        public async void Increment_count_success()
        {
            // Arrange
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
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
            var info = new TweetProcessor(repo, _mockLogger.Object);
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
            var info = new TweetProcessor(repo, _mockLogger.Object);
            var tweet = BuildTestTweet("Test");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(0, await repo.GetPercentWithEmoji());
        }
        
        [Fact]
        public async void Emoji_produces_emoji()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
            var tweet = BuildTestTweet("❤");
            
            info.ProcessTweet(tweet);
            
            Assert.Equal(100, await repo.GetPercentWithEmoji());
        }
        
        [Fact]
        public async void Calculate_emoji_rate_success()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
        
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
            var info = new TweetProcessor(repo, _mockLogger.Object);
        
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
            var info = new TweetProcessor(repo, _mockLogger.Object);
            var hashtags = new[]
            {
                "#Blessed",
                "#Mission"
            };
            var tweet = BuildTestTweet("#Blessed #Mission", "temp", hashtags);
        
            info.ProcessTweet(tweet);
        
            var result = await repo.GetTopHashtag(2);
            
            Assert.Equal(2, result.Count());
        }
        
        [Fact]
        public async void Identify_tweets_with_url()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
            var tweet = BuildTestTweet("Test", "https://www.google.com");
            
            info.ProcessTweet(tweet);
        
            var result = await repo.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }
        
        [Fact]
        public async void Identify_tweets_with_image_url()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
            var tweet = BuildTestTweet("Test", "http://pic.twitter.com/ABCDEFG");
            
            info.ProcessTweet(tweet);
        
            var result = await repo.GetPercentWithUrl();
            
            Assert.Equal(100, result);
        }
        
        [Fact]
        public async void Calculate_percentage_of_urls()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
        
            foreach (var tweet in BuildListOfTweets())
            {
                info.ProcessTweet(tweet);
            }
        
            var percentWithUrl = await repo.GetPercentWithUrl();
            var percentWithUrlImg = await repo.GetPercentWithUrlOfPhoto();
            
            Assert.Equal(67, percentWithUrl);
            Assert.Equal(17, percentWithUrlImg);
        }
        
        [Fact]
        public async void Urls_Count_Success()
        {
            var repo = new TweetInfoRepository();
            var info = new TweetProcessor(repo, _mockLogger.Object);
        
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