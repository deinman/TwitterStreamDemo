using System.Collections;

namespace TwitterStats.API.DTO
{
    public class HashtagDto
    {
        public IEnumerable TopHashtags { get; init; } = null!;
    }
}