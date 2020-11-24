namespace AwsAppConfigClient
{
    public class Secrets
    {
        public string AwsKey { get; }

        public string AwsSecret { get; }

        public Secrets(string awsKey, string awsSecret)
        {
            AwsKey = awsKey;
            AwsSecret = awsSecret;
        }
    }
}