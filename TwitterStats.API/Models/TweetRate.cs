namespace TwitterStats.API.Models
{
    public class TweetRate
    {
        public int TweetsPerHour => TweetsPerMinute * 60;
        public int TweetsPerMinute => TweetsPerSecond * 60; 
        //public int TweetsPerHour { get; set; }
        //public int TweetsPerMinute { get; set; }
        public int TweetsPerSecond { get; set; }
    }
}