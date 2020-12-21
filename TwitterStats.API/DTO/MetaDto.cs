namespace TwitterStats.API.DTO
{
    public class MetaDto
    {
        public CountDto CountInfo { get; init; } = null!;
        public TweetRateDto TweetRate { get; init; } = null!;
        public EmojiDto EmojiInfo { get; init; } = null!;
        public HashtagDto HashtagInfo { get; init; } = null!;
        public UrlDto UrlInfo { get; init; } = null!;
    }
}