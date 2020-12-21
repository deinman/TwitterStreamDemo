using System.Collections;

namespace TwitterStats.API.DTO
{
    public class UrlDto
    {
        public int PercentWithUrl { get; init; }
        public int PercentWithUrlOfImage { get; init; }
        public IEnumerable TopDomainsOfUrls { get; init; } = null!;
    }
}