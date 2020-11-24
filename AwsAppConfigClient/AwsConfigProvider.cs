using System;
using System.Text;
using System.Threading.Tasks;
using Amazon.AppConfig;
using Amazon.AppConfig.Model;
using AwsAppConfigClient.Logger;
using Environment = Amazon.AppConfig.Model.Environment;

namespace AwsAppConfigClient
{
    public class AwsConfigProvider
    {
        private readonly string _clientId;
        private readonly Application _application;
        private readonly Environment _environment;
        private readonly ConfigurationProfileSummary _configurationProfile;
        private readonly ConsoleLogger _logger;
        private readonly AmazonAppConfigClient _client;

        private string _currentVersion;
        private string _currentConfig;

        public AwsConfigProvider(string clientId, AmazonAppConfigClient client, Application application, Environment environment,
            ConfigurationProfileSummary configurationProfile, ConsoleLogger logger)
        {
            _clientId = clientId;
            _application = application;
            _environment = environment;
            _configurationProfile = configurationProfile;
            _logger = logger;
            _client = client;
            _currentVersion = null;
            _currentConfig = null;
        }

        public async Task<string> GetConfig()
        {
            var config = await _client.GetConfigurationAsync(new GetConfigurationRequest
            {
                Application = _application.Id,
                Environment = _environment.Id,
                Configuration = _configurationProfile.Id,
                ClientId = _clientId,
                ClientConfigurationVersion = _currentVersion
            });

            var cfgIsUpToDate = config.Content == null || config.ContentLength == 0;

            if (cfgIsUpToDate)
            {
                if (string.IsNullOrWhiteSpace(_currentConfig))
                    throw new Exception("Fatal error. Aws returned empty content which means we requested a config with version and must to have cached value for this version, " +
                                        $"but we don't have. ClientId: {_clientId}; Application: {_application.Id}; Environment: {_environment.Id}; " +
                                        $"Configuration: {_configurationProfile.Id}; ClientConfigurationVersion: {_currentVersion};");


                _logger.WriteConsistently(() =>
                {
                    Console.Write($"{_clientId}, configuration {_configurationProfile.Name}. ");
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write($"Returns preserved version {_currentVersion}. \r\n");
                });

                return _currentConfig;
            }

            _currentVersion = config.ConfigurationVersion;
            _currentConfig = Encoding.ASCII.GetString(config.Content.ToArray());

            _logger.WriteConsistently(() =>
            {
                Console.Write($"{_clientId}, configuration {_configurationProfile.Name}. ");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write($"Returns remote version {_currentVersion}. \r\n");
            });

            return _currentConfig;
        }
    }
}