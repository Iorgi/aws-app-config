namespace AwsAppConfigClient
{
    public class AppConfigProviderSpec
    {
        public string ClientId { get; }

        public string ApplicationName { get; }

        public string EnvironmentName { get; }

        public string ConfigurationName { get; }

        public AppConfigProviderSpec(string clientId, string applicationName, string environmentName, string configurationName)
        {
            ClientId = clientId;
            ApplicationName = applicationName;
            EnvironmentName = environmentName;
            ConfigurationName = configurationName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ClientId != null ? ClientId.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (ApplicationName != null ? ApplicationName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EnvironmentName != null ? EnvironmentName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConfigurationName != null ? ConfigurationName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}