namespace AwsAppConfigClient
{
    public class StartArguments
    {
        public string Application { get; set; }

        public string Environment { get; set; }

        public int ClientsCount { get; set; } = 1;
    }
}