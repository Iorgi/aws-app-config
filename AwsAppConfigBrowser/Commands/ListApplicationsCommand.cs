using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using AwsAppConfig.Common.Commands;

namespace AwsAppConfigBrowser.Commands
{
    public class ListApplicationsCommand : ICommand
    {
        private readonly AmazonAppConfigClient _client;

        public ListApplicationsCommand(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<bool> Run()
        {
            var apps = await _client.ListApplicationsAsync(new ListApplicationsRequest());
            Console.WriteLine(string.Join(";", apps.Items.Select(x => x.Name)));

            return false;
        }
    }
}