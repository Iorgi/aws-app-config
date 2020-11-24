using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;

namespace AwsAppConfigDeployment.Services
{
    public class DeploymentService
    {
        private readonly AmazonAppConfigClient _client;

        public DeploymentService(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<StartDeploymentResponse> StartDeployment(string appId, string envId, string deploymentStrategyId, string cfgId, string versionNumber)
        {
            var deploymentResult = await _client.StartDeploymentAsync(new StartDeploymentRequest
            {
                ApplicationId = appId,
                EnvironmentId = envId,
                ConfigurationProfileId = cfgId,
                ConfigurationVersion = versionNumber,
                DeploymentStrategyId = deploymentStrategyId
            });

            Console.WriteLine($"Deployment for new configuration with Version: [{versionNumber}] started. Deployment number: [{deploymentResult.DeploymentNumber}]");

            return deploymentResult;
        }

        public async Task WaitDeployment(StartDeploymentResponse deploymentResult)
        {
            while (true)
            {
                var actualDeployment = await _client.GetDeploymentAsync(new GetDeploymentRequest()
                {
                    ApplicationId = deploymentResult.ApplicationId,
                    EnvironmentId = deploymentResult.EnvironmentId,
                    DeploymentNumber = deploymentResult.DeploymentNumber
                });

                Console.WriteLine($"Deployment is {actualDeployment.State}");

                if (actualDeployment.State == DeploymentState.COMPLETE)
                    break;

                if (actualDeployment.State == DeploymentState.ROLLED_BACK)
                    throw new Exception("An error occured during deployment, check management console for details.");

                Thread.Sleep(5000);
            }
        }
    }
}