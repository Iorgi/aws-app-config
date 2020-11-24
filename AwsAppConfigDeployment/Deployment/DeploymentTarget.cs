using System.Collections.Generic;
using Newtonsoft.Json;

namespace AwsAppConfigDeployment.Deployment
{
    public class DeploymentTarget
    {
        [JsonProperty("application")]
        public string ApplicationName { get; set; }

        [JsonProperty("environment")]
        public string EnvironmentName { get; set; }

        [JsonProperty("deploymentStrategy")]
        public string DeploymentStrategy { get; set; }

        [JsonProperty("configurations")]
        public List<Configuration>Configurations { get; set; }
    }
}