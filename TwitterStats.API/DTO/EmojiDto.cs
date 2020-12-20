using System.Collections;

namespace TwitterStats.API.DTO
{
    public class EmojiDto
    {
        public IEnumerable TopEmoji { get; set; }
        public int Percentage { get; set; }
    }
}