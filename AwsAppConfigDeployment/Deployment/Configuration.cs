
using Newtonsoft.Json;

namespace AwsAppConfigDeployment.Deployment
{
    public class Configuration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }
    }
}