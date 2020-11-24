using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using AwsAppConfig.Common.Commands;
using AwsAppConfig.Common.Services;
using AwsAppConfigDeployment.Deployment;
using AwsAppConfigDeployment.Services;
using Newtonsoft.Json;
using Environment = Amazon.AppConfig.Model.Environment;

namespace AwsAppConfigDeployment.Commands
{
    public class DeploymentCommand : ICommand
    {
        private readonly AmazonAppConfigClient _client;
        private readonly DeploymentService _deploymentService;
        private readonly AwsAppConfigApplicationService _applicationService;
        private readonly AwsAppConfigEnvironmentService _environmentService;
        private readonly AwsAppConfigConfigurationProfileService _configurationProfileService;

        public DeploymentCommand(AmazonAppConfigClient client)
        {
            _client = client;
            _deploymentService = new DeploymentService(client);
            _applicationService = new AwsAppConfigApplicationService(client);
            _environmentService = new AwsAppConfigEnvironmentService(client);
            _configurationProfileService = new AwsAppConfigConfigurationProfileService(client);
        }

        public async Task<bool> Run()
        {
            var manifestData = GetDeploymentManifest();

            Console.Write("Deployment environment: ");
            var deploymentEnvironment = Console.ReadLine();

            foreach (var deploymentTarget in manifestData.Manifest.Targets.Where(x => x.EnvironmentName == deploymentEnvironment))
            {
                Console.WriteLine($"Starting deployment for application [{deploymentTarget.ApplicationName}] and environment [{deploymentTarget.EnvironmentName}]");

                var setup = await GetDeploymentSetup(deploymentTarget);

                foreach (var configuration in deploymentTarget.Configurations)
                {
                    Console.WriteLine($"Starting deployment for configuration {configuration.Name}");

                    string cfgProfileId = await GetConfigurationProfileSummary(setup.Application, configuration.Name);

                    var dataFilePath = Path.Combine(manifestData.ManifestParentDirectory, configuration.File);
                    if (!File.Exists(dataFilePath))
                    {
                        Console.WriteLine($"Can't find file specified: [{dataFilePath}]'");
                        return false;
                    }

                    var content = Encoding.ASCII.GetBytes(File.ReadAllText(dataFilePath));

                    using (var dataStream = new MemoryStream(content))
                    {
                        var createVersionResult = await _client.CreateHostedConfigurationVersionAsync(
                            new CreateHostedConfigurationVersionRequest
                            {
                                ApplicationId = setup.Application.Id,
                                ConfigurationProfileId = cfgProfileId,
                                Content = dataStream,
                                ContentType = "text/plain"
                            });

                        Console.WriteLine($"New configuration version created. Version: [{createVersionResult.VersionNumber}]");

                        var deploymentStartResult = await _deploymentService.StartDeployment(setup.Application.Id, setup.Environment.Id,
                            setup.DeploymentStrategy.Id, cfgProfileId, createVersionResult.VersionNumber.ToString());

                        await _deploymentService.WaitDeployment(deploymentStartResult);
                    }

                    Console.WriteLine($"Deployment for configuration {configuration.Name} finished.");
                }

                Console.WriteLine($"Deployment for application [{deploymentTarget.ApplicationName}] and environment [{deploymentTarget.EnvironmentName}] finished.");
            }

            Console.WriteLine($"Deployment using manifest [{manifestData.ManifestFilePath}] finished.");

            return false;
        }

        private (DeploymentManifest Manifest, string ManifestParentDirectory, string ManifestFilePath) GetDeploymentManifest()
        {
            Console.Write("Deployment manifest file path: ");
            var filePath = Console.ReadLine();

            if (!File.Exists(filePath))
                throw new Exception($"Can't find file specified: [{filePath}]'");

            var manifestFileInfo = new FileInfo(filePath);
            var manifestParentDirectory = manifestFileInfo.Directory.FullName;
            var manifest = JsonConvert.DeserializeObject<DeploymentManifest>(File.ReadAllText(filePath));

            return (manifest, manifestParentDirectory, filePath);
        }

        private async Task<(Application Application, Environment Environment, DeploymentStrategy DeploymentStrategy)> 
            GetDeploymentSetup(DeploymentTarget deploymentTarget)
        {
            var app = await _applicationService.Get(deploymentTarget.ApplicationName);
            if (app == null)
                throw new Exception($"Can't find application [{deploymentTarget.ApplicationName}]. Check the correctness of the manifest.");

            var env = await _environmentService.Get(app.Id, deploymentTarget.EnvironmentName);
            if (env == null)
                throw new Exception($"Can't find environment [{deploymentTarget.EnvironmentName}] for application [{deploymentTarget.ApplicationName}]. Check the correctness of the manifest.");

            var deploymentStrategies = await _client.ListDeploymentStrategiesAsync(new ListDeploymentStrategiesRequest());
            var deploymentStrategy =
                deploymentStrategies.Items.FirstOrDefault(x => x.Name == deploymentTarget.DeploymentStrategy);
            if (deploymentStrategy == null)
                throw new Exception($"Can't find deployment strategy [{deploymentTarget.DeploymentStrategy}]. Check the correctness of the manifest.");

            return (app, env, deploymentStrategy);
        }

        private async Task<string> GetConfigurationProfileSummary(Application application, string configurationName)
        {
            var cfgProfile = await _configurationProfileService.Get(application.Id, configurationName);
            if (cfgProfile == null)
            {
                Console.WriteLine($"Can't find configuration [{configurationName}] for application [{application.Name}]. Creating the profile");

                var newProfile = await _client.CreateConfigurationProfileAsync(new CreateConfigurationProfileRequest
                {
                    ApplicationId = application.Id,
                    Name = configurationName,
                    LocationUri = "hosted"
                });
                return newProfile.Id;
            }

            return cfgProfile.Id;
        }

        
    }
}