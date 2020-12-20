using System.Collections;

namespace TwitterStats.API.DTO
{
    public class UrlDto
    {
        public int PercentWithUrl { get; set; }
        public int PercentWithUrlOfImage { get; set; }
        public IEnumerable TopDomainsOfUrls { get; set; }
    }
}