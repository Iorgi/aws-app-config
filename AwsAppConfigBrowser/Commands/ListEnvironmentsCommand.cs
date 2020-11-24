using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using AwsAppConfig.Common.Commands;

namespace AwsAppConfigBrowser.Commands
{
    public class ListEnvironmentsCommand : ICommand
    {
        private readonly AmazonAppConfigClient _client;

        public ListEnvironmentsCommand(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<bool> Run()
        {
            Console.Write("Application name: ");
            var promptedApp = Console.ReadLine();
            Console.WriteLine($"Environments for application {promptedApp}: ");
            var apps = await _client.ListApplicationsAsync(new ListApplicationsRequest());
            var app = apps.Items.FirstOrDefault(x => x.Name == promptedApp);
            if (app == null)
            {
                Console.WriteLine($"Can't find application with name [{promptedApp}]");
                return false;
            }

            var envs = await _client.ListEnvironmentsAsync(new ListEnvironmentsRequest
            {
                ApplicationId = app.Id
            });

            foreach (var env in envs.Items)
            {
                Console.WriteLine(env.Name);
            }

            return false;
        }
    }
}