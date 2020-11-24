using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using AwsAppConfig.Common.Commands;
using AwsAppConfigDeployment.Services;

namespace AwsAppConfigDeployment.Commands
{
    public class GetConfigurationCommand : ICommand
    {
        private readonly AmazonAppConfigClient _client;
        private readonly UserPollService _userPollService;

        public GetConfigurationCommand(AmazonAppConfigClient client)
        {
            _client = client;
            _userPollService = new UserPollService(client);
        }

        public async Task<bool> Run()
        {
            var app = await _userPollService.AskApplication();
            var env = await _userPollService.AskEnvironment(app);
            var cfgProfile = await _userPollService.AskConfigurationProfile(app);

            Console.Write("Client id: ");
            var clientId = Console.ReadLine();

            var version = _userPollService.AskVersion();

            var config = await _client.GetConfigurationAsync(new GetConfigurationRequest
            {
                Application = app.Id,
                Environment = env.Id,
                Configuration = cfgProfile.Id,
                ClientId = clientId,
                ClientConfigurationVersion = version
            });

            Console.WriteLine($"Config version: {config.ConfigurationVersion}");
            Console.WriteLine($"Config content type: {config.ContentType}");

            var strContent = Encoding.ASCII.GetString(config.Content.ToArray());
            Console.WriteLine("Config content:");
            Console.WriteLine(strContent);


            return false;
        }
    }
}