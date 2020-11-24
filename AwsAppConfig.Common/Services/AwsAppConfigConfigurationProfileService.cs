using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;

namespace AwsAppConfig.Common.Services
{
    public class AwsAppConfigConfigurationProfileService
    {
        private readonly AmazonAppConfigClient _client;

        public AwsAppConfigConfigurationProfileService(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<ConfigurationProfileSummary> Get(string appId, string cfgName)
        {
            var cfgProfiles = await _client.ListConfigurationProfilesAsync(new ListConfigurationProfilesRequest
            {
                ApplicationId = appId
            });
            return cfgProfiles.Items.FirstOrDefault(x => x.Name == cfgName);
        }
    }
}