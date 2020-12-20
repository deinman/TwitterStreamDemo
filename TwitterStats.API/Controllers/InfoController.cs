using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterStats.API.DTO;
using TwitterStats.API.Repository;

namespace TwitterStats.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ITweetInfoRepository _info;
        // No logger since this controller only serves up queries and Serilog's request logging captures that info.

        public InfoController(ITweetInfoRepository info)
        {
            _info = info;
        }

        [HttpGet("Count")]
        public async Task<ActionResult<CountDto>> GetCount()
        {
            var ret = new CountDto
            {
                TotalTweets = await _info.GetCount()
            };

            return Ok(ret);
        }

        [HttpGet("Rate")]
        public async Task<ActionResult<TweetRateDto>> GetTweetRate()
        {
            var ret = new TweetRateDto
            {
                Rate = await _info.GetTweetRate()
            };

            return Ok(ret);
        }

        [HttpGet("Emoji")]
        public async Task<ActionResult<EmojiDto>> GetEmojiInfo([FromQuery] int count = 10)
        {
            var ret = new EmojiDto
            {
                TopEmoji = await _info.GetTopEmoji(count),
                Percentage = await _info.GetPercentWithEmoji()
            };

            return Ok(ret);
        }

        [HttpGet("Hashtag")]
        public async Task<ActionResult<HashtagDto>> GetHashtagInfo([FromQuery] int count = 10)
        {
            var ret = new HashtagDto
            {
                TopHashtags = await _info.GetTopHashtag(count)
            };

            return Ok(ret);
        }

        [HttpGet("Urls")]
        public async Task<ActionResult<UrlDto>> GetUrlInfo([FromQuery] int count = 10)
        {
            var ret = new UrlDto
            {
                PercentWithUrl = await _info.GetPercentWithUrl(),
                PercentWithUrlOfImage = await _info.GetPercentWithUrlOfPhoto(),
                TopDomainsOfUrls = await _info.GetTopDomain(count)
            };

            return Ok(ret);
        }

        [HttpGet("All")]
        public async Task<ActionResult<MetaDto>> GetAllInfo([FromQuery] int count = 10)
        {
            var ret = new MetaDto
            {
                CountInfo = new CountDto
                {
                    TotalTweets = await _info.GetCount()
                },
                TweetRate = new TweetRateDto
                {
                    Rate = await _info.GetTweetRate()
                },
                EmojiInfo = new EmojiDto
                {
                    Percentage = await _info.GetPercentWithEmoji(),
                    TopEmoji = await _info.GetTopEmoji(count)
                },
                HashtagInfo = new HashtagDto
                {
                    TopHashtags = await _info.GetTopHashtag(count)
                },
                UrlInfo = new UrlDto
                {
                    PercentWithUrl = await _info.GetPercentWithUrl(),
                    PercentWithUrlOfImage = await _info.GetPercentWithUrlOfPhoto(),
                    TopDomainsOfUrls = await _info.GetTopDomain(count)
                }
            };

            return Ok(ret);
        }
    }
}