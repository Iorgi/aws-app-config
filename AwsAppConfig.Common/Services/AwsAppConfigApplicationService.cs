using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;

namespace AwsAppConfig.Common.Services
{
    public class AwsAppConfigApplicationService
    {
        private readonly AmazonAppConfigClient _client;

        public AwsAppConfigApplicationService(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<Application> Get(string appName)
        {
            var apps = await _client.ListApplicationsAsync(new ListApplicationsRequest());
            return apps.Items.FirstOrDefault(x => x.Name == appName);
        }
    }
}