using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TwitterStats.API.DTO;
using TwitterStats.API.Services;

namespace TwitterStats.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IProcessTweetInfo _info;

        public InfoController(ILogger<InfoController> logger, IProcessTweetInfo info)
        {
            _logger = logger;
            _info = info;
        }

        [HttpGet("Count")]
        public async Task<ActionResult<CountDto>> GetCount()
        {
            var ret = new CountDto
            {
                TotalTweets = _info.GetCount()
            };

            return Ok(ret);
        }

        [HttpGet("Rate")]
        public async Task<ActionResult<TweetRateDto>> GetTweetRate()
        {
            var ret = new TweetRateDto
            {
                Rate = _info.GetTweetRate()
            };

            return Ok(ret);
        }

        [HttpGet("Emoji")]
        public async Task<ActionResult<EmojiDto>> GetEmojiInfo([FromQuery]int count = 10)
        {
            var ret = new EmojiDto
            {
                TopEmoji = _info.GetTopEmoji(count),
                Percentage = _info.GetPercentWithEmoji()
            };

            return Ok(ret);
        }

        [HttpGet("Hashtag")]
        public async Task<ActionResult<HashtagDto>> GetHashtagInfo([FromQuery] int count = 10)
        {
            var ret = new HashtagDto
            {
                TopHashtags = _info.GetTopHashtag(count)
            };

            return Ok(ret);
        }

        [HttpGet("Urls")]
        public async Task<ActionResult<UrlDto>> GetUrlInfo([FromQuery] int count = 10)
        {
            var ret = new UrlDto
            {
                PercentWithUrl = _info.GetPercentWithUrl(),
                PercentWithUrlOfImage = _info.GetPercentWithUrlOfPhoto(),
                TopDomainsOfUrls = _info.GetTopDomain(count)
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
                    TotalTweets = _info.GetCount()
                },
                TweetRate = new TweetRateDto
                {
                    Rate = _info.GetTweetRate()
                },
                EmojiInfo = new EmojiDto
                {
                    Percentage = _info.GetPercentWithEmoji(),
                    TopEmoji = _info.GetTopEmoji(count)
                },
                HashtagInfo = new HashtagDto
                {
                    TopHashtags = _info.GetTopHashtag(count)
                },
                UrlInfo = new UrlDto
                {
                    PercentWithUrl = _info.GetPercentWithUrl(),
                    PercentWithUrlOfImage = _info.GetPercentWithUrlOfPhoto(),
                    TopDomainsOfUrls = _info.GetTopDomain(count)
                }
            };

            return Ok(ret);
        }
    }
}