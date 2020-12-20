namespace TwitterStats.API.Models
{
    public class TweetRate
    {
        public int TweetsPerHour => TweetsPerMinute * 60;
        public int TweetsPerMinute => TweetsPerSecond * 60;
        public int TweetsPerSecond { get; set; }
    }
}