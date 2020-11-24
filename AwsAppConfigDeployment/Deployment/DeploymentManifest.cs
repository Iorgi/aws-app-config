using System.Collections.Generic;
using Newtonsoft.Json;

namespace AwsAppConfigDeployment.Deployment
{
    public class DeploymentManifest
    {
        [JsonProperty("targets")]
        public List<DeploymentTarget> Targets { get; set; }
    }
}