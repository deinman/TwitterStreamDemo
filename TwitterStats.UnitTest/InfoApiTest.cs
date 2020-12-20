using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TwitterStats.API.Controllers;
using TwitterStats.API.DTO;
using TwitterStats.API.Models;
using TwitterStats.API.Repository;
using Xunit;

namespace TwitterStats.UnitTest
{
    /*
     * In the interest of time I'm not actually going to finish these out. You get the idea though.
     * This set of tests should simply be a sanity-check for our controller.
     * Since the controller is really just a query handler, I would argue that unit tests here are unnecessary.
     * Just wanted to show how I'd get these tested in interest of 100% test coverage.
     */
    public class InfoApiTest
    {
        private readonly Mock<ITweetInfoRepository> _repoMock;

        public InfoApiTest()
        {
            _repoMock = new Mock<ITweetInfoRepository>();
        }

        private InfoController BuildInfoController()
        {
            return new(_repoMock.Object);
        }

        [Fact]
        public async void Get_count_success()
        {
            // Arrange
            long expectation = 256;
            _repoMock.Setup(x => x.GetCount()).Returns(Task.FromResult(expectation));

            // Act
            var infoController = BuildInfoController();
            var result = await infoController.GetCount();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<CountDto>(objectResult.Value);
            Assert.Equal(expectation, dto.TotalTweets);
        }

        [Fact]
        public async void Get_rate_success()
        {
            // Arrange
            var expectation = new TweetRate
            {
                TweetsPerSecond = 60
            };
            _repoMock.Setup(x => x.GetTweetRate()).Returns(Task.FromResult(expectation));

            // Act
            var infoController = BuildInfoController();
            var result = await infoController.GetTweetRate();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TweetRateDto>(objectResult.Value);
            Assert.Equal(expectation, dto.Rate);
        }

        [Fact]
        public async void Get_emoji_success()
        {
            // Arrange
            var itemMock = new Mock<IEnumerable<KeyValuePair<string, int>>>();
            _repoMock.Setup(x => x.GetTopEmoji(1)).Returns(Task.FromResult(itemMock.Object));

            // Act
            var infoController = BuildInfoController();
            var result = await infoController.GetEmojiInfo();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<EmojiDto>(objectResult.Value);
        }
    }
}