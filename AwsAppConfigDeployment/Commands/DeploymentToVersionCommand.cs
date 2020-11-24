using System;
using System.Threading.Tasks;
using Amazon.AppConfig;
using AwsAppConfig.Common.Commands;
using AwsAppConfigDeployment.Services;

namespace AwsAppConfigDeployment.Commands
{
    public class DeploymentToVersionCommand : ICommand
    {
        private readonly UserPollService _userPollService;
        private readonly DeploymentService _deploymentService;

        public DeploymentToVersionCommand(AmazonAppConfigClient client)
        {
            _userPollService = new UserPollService(client);
            _deploymentService = new DeploymentService(client);
        }

        public async Task<bool> Run()
        {
            var app = await _userPollService.AskApplication();
            var env = await _userPollService.AskEnvironment(app);
            var cfgProfile = await _userPollService.AskConfigurationProfile(app);
            var strategy = await _userPollService.AskDeploymentStrategy();
            var version = await _userPollService.AskVersion(app.Id, cfgProfile.Id);
            var startDeploymentResult = await 
                _deploymentService.StartDeployment(app.Id, env.Id, strategy.Id, cfgProfile.Id, version);
            await _deploymentService.WaitDeployment(startDeploymentResult);

            Console.WriteLine("Deployment finished");
            return false;
        }
    }
}