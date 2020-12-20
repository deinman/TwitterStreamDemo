namespace TwitterStats.API.DTO
{
    public class MetaDto
    {
        public CountDto CountInfo { get; set; }
        public TweetRateDto TweetRate { get; set; }
        public EmojiDto EmojiInfo { get; set; }
        public HashtagDto HashtagInfo { get; set; }
        public UrlDto UrlInfo { get; set; }
    }
}