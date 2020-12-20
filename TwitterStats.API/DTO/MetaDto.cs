using TwitterStats.API.Models;

namespace TwitterStats.API.DTO
{
    public class MetaDto
    {
        public long TotalTweets { get; set; }
        public TweetRate Rates { get; set; }
        public EmojiDto EmojiInfo { get; set; }
        public TweetRateDto TweetRate { get; set; }
        public CountDto CountInfo { get; set; }
        public HashtagDto HashtagInfo { get; set; }
        public UrlDto UrlInfo { get; set; }
    }
}