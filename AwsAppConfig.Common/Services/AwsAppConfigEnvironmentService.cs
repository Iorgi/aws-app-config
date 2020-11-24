using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;

namespace AwsAppConfig.Common.Services
{
    public class AwsAppConfigEnvironmentService
    {
        private readonly AmazonAppConfigClient _client;

        public AwsAppConfigEnvironmentService(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<Environment> Get(string appId, string envName)
        {
            var envs = await _client.ListEnvironmentsAsync(new ListEnvironmentsRequest
            {
                ApplicationId = appId
            });
            return envs.Items.FirstOrDefault(x => x.Name == envName);
        }
    }
}