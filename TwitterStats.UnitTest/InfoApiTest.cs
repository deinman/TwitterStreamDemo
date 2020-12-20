using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TwitterStats.API.Controllers;
using TwitterStats.API.DTO;
using TwitterStats.API.Models;
using TwitterStats.API.Services;
using Xunit;

namespace TwitterStats.UnitTest
{
    public class InfoApiTest
    {
        private readonly Mock<ILogger<InfoController>> _loggerMock;
        private readonly Mock<IProcessTweetInfo> _processMock;

        public InfoApiTest()
        {
            _loggerMock = new Mock<ILogger<InfoController>>();
            _processMock = new Mock<IProcessTweetInfo>();
        }

        private InfoController BuildInfoController()
        {
            return new InfoController(_loggerMock.Object, _processMock.Object);
        }

        [Fact]
        public async void Get_count_success()
        {
            // Arrange
            long expectation = 256;
            _processMock.Setup(x => x.GetCount()).Returns(expectation);
            
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
            _processMock.Setup(x => x.GetTweetRate()).Returns(expectation);
            
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
            var expectation = new TweetRate
            {
                TweetsPerSecond = 60
            };
            _processMock.Setup(x => x.GetTweetRate()).Returns(expectation);
            
            // Act
            var infoController = BuildInfoController();
            var result = await infoController.GetTweetRate();
            
            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TweetRateDto>(objectResult.Value);
        }

        // [Fact]
        // public async void Get_meta_success()
        // {
        //     // Arrange
        //     long countExpectation = 256;
        //     var rateExpectation = new TweetRate
        //     {
        //         TweetsPerSecond = 60
        //     };
        //     _processMock.Setup(x => x.GetCount()).Returns(countExpectation);
        //     _processMock.Setup(x => x.GetTweetRate()).Returns(rateExpectation);
        //     
        //     // Act
        //     var infoController = BuildInfoController();
        //     var result = await infoController.GetAllInfo();
        //     
        //     // Assert
        //     var objectResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var dto = Assert.IsType<MetaDto>(objectResult.Value);
        //     Assert.Equal(countExpectation, dto.TotalTweets);
        //     Assert.Equal(rateExpectation, dto.Rates);
        // }
    }
}