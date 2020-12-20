using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterStats.API.Services
{
    public interface ICredentialProvider
    {
        /// <summary>
        /// Uses credentials in IConfiguration to build an authenticated Twitter client.
        /// </summary>
        /// <returns></returns>
        public Task<TwitterClient> GetAuthenticatedTwitterClient();
    }
}