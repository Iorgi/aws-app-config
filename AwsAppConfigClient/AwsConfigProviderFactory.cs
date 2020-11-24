using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Amazon.AppConfig;
using AwsAppConfig.Common.Services;
using AwsAppConfigClient.Logger;

namespace AwsAppConfigClient
{
    public class AwsConfigProviderFactory
    {
        private readonly ConsoleLogger _logger;
        private readonly AmazonAppConfigClient _client;
        private readonly AwsAppConfigApplicationService _applicationService;
        private readonly AwsAppConfigEnvironmentService _environmentService;
        private readonly AwsAppConfigConfigurationProfileService _configurationProfileService;

        public AwsConfigProviderFactory(Secrets credentials, ConsoleLogger logger)
        {
            _logger = logger;
            _client = new AmazonAppConfigClient(credentials.AwsKey, credentials.AwsSecret);
            _applicationService = new AwsAppConfigApplicationService(_client);
            _environmentService = new AwsAppConfigEnvironmentService(_client);
            _configurationProfileService = new AwsAppConfigConfigurationProfileService(_client);
        }

        private static readonly ConcurrentDictionary<AppConfigProviderSpec, AwsConfigProvider> AwsConfigProviders = new ConcurrentDictionary<AppConfigProviderSpec, AwsConfigProvider>();

        public async Task<AwsConfigProvider> CreateAsync(AppConfigProviderSpec specification)
        {
            var application = await _applicationService.Get(specification.ApplicationName);
            var environment = await _environmentService.Get(application.Id, specification.EnvironmentName);
            var configurationProfile = await _configurationProfileService.Get(application.Id, specification.ConfigurationName);

            return AwsConfigProviders.GetOrAdd(specification, settings => new AwsConfigProvider(specification.ClientId, _client, 
                application, environment, configurationProfile, _logger));
        }
    }
}