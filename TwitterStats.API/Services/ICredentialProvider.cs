using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterStats.API.Services
{
    public interface ICredentialProvider
    {
        public Task<TwitterClient> GetAuthenticatedTwitterClient();
    }
}