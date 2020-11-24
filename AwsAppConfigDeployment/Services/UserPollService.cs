using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using Environment = Amazon.AppConfig.Model.Environment;

namespace AwsAppConfigDeployment.Services
{
    public class UserPollService
    {
        private readonly AmazonAppConfigClient _client;

        public UserPollService(AmazonAppConfigClient client)
        {
            _client = client;
        }

        public async Task<Application> AskApplication()
        {
            var apps = await _client.ListApplicationsAsync(new ListApplicationsRequest());

            Console.Write($"Application name (available, {string.Join(";", apps.Items.Select(x => $"[{x.Name}]"))}): ");
            var promptedApp = Console.ReadLine();
            var app = apps.Items.FirstOrDefault(x => x.Name == promptedApp);
            if (app == null)
                throw new Exception($"Can't find application with name [{promptedApp}]");

            return app;
        }

        public async Task<Environment> AskEnvironment(Application application)
        {
            var envs = await _client.ListEnvironmentsAsync(new ListEnvironmentsRequest
            {
                ApplicationId = application.Id
            });
            Console.Write($"Environment name (available, {string.Join(";", envs.Items.Select(x => $"[{x.Name}]"))}): ");
            var promptedEnv = Console.ReadLine();
            var env = envs.Items.FirstOrDefault(x => x.Name == promptedEnv);
            if (env == null)
                throw new Exception($"Can't find environment [{promptedEnv}] for application [{application.Name}]");

            return env;
        }

        public async Task<ConfigurationProfileSummary> AskConfigurationProfile(Application application)
        {
            var configs = await _client.ListConfigurationProfilesAsync(new ListConfigurationProfilesRequest
            {
                ApplicationId = application.Id
            });
            Console.Write($"Config name (available, {string.Join(";", configs.Items.Select(x => $"[{x.Name}]"))}): ");
            var promptedCfgProfile = Console.ReadLine();
            var cfgProfile = configs.Items.FirstOrDefault(x => x.Name == promptedCfgProfile);
            if (cfgProfile == null)
                throw new Exception($"Can't find configuration profile [{promptedCfgProfile}] for application [{application.Name}]");

            return cfgProfile;
        }

        public string AskVersion()
        {
            Console.Write("Version: ");
            var version = Console.ReadLine();
            if (version != null && version.Length == 0)
                version = null;

            return version;
        }

        public async Task<string> AskVersion(string appId, string cfgProfileId)
        {
            var versions = await _client.ListHostedConfigurationVersionsAsync(new ListHostedConfigurationVersionsRequest
            {
                ApplicationId = appId,
                ConfigurationProfileId = cfgProfileId
            });

            Console.Write($"Version (available, {string.Join(";", versions.Items.Select(x => $"[{x.VersionNumber}]"))}): ");
            var promptedVersion = Console.ReadLine();
            var version =
                versions.Items.FirstOrDefault(x => x.VersionNumber.ToString() == promptedVersion);
            if (version == null)
                throw new Exception($"Can't find version [{promptedVersion}]");

            return version.VersionNumber.ToString();
        }

        public async Task<DeploymentStrategy> AskDeploymentStrategy()
        {
            var deploymentStrategies = await _client.ListDeploymentStrategiesAsync(new ListDeploymentStrategiesRequest());
            Console.Write($"Deployment strategy (available, {string.Join(";", deploymentStrategies.Items.Select(x => $"[{x.Name}]"))}): ");
            var promptedStrategy = Console.ReadLine();
            var deploymentStrategy =
                deploymentStrategies.Items.FirstOrDefault(x => x.Name == promptedStrategy);
            if (deploymentStrategy == null)
                throw new Exception($"Can't find deployment strategy [{promptedStrategy}]");

            return deploymentStrategy;
        }
    }
}