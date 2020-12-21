using System.Collections;

namespace TwitterStats.API.DTO
{
    public class EmojiDto
    {
        public IEnumerable TopEmoji { get; init; } = null!;
        public int Percentage { get; init; }
    }
}