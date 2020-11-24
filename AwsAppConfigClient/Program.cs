using System;
using System.Threading;
using System.Threading.Tasks;
using AwsAppConfigClient.Logger;

namespace AwsAppConfigClient
{
    class Program
    {
        private static readonly AwsConfigProviderFactory AwsConfigProviderFactory = 
            new AwsConfigProviderFactory(new Secrets("", ""), new ConsoleLogger());
        
        static void Main(string[] args)
        {
            var startArguments = ParseArguments(args);
            var cts = new CancellationTokenSource();
            var tasks = new Task[startArguments.ClientsCount];

            Console.WriteLine("Press any key to exit...");

            for (int i = 0; i < startArguments.ClientsCount; i++)
            {
                tasks[i] = StartClient(startArguments.Application, startArguments.Environment, cts.Token, i);
            }

            Console.ReadKey();

            cts.Cancel();
        }

        private static Task StartClient(string applicationName, string environmentName, CancellationToken cancellationToken, int clientNumber)
        {
            return Task.Run(async () =>
            {
                var clientId = $"client-{applicationName}-{environmentName}-{clientNumber}";
                Console.WriteLine($"{clientId} started.");

                var rscCfgProvider = await AwsConfigProviderFactory.CreateAsync(new AppConfigProviderSpec(clientId, applicationName, environmentName, $"{environmentName}.RSC.Config"));
                var rscGgCfgProvider = await AwsConfigProviderFactory.CreateAsync(new AppConfigProviderSpec(clientId, applicationName, environmentName, $"{environmentName}.RSC.GG.Config"));
                var rscRwaCfgProvider = await AwsConfigProviderFactory.CreateAsync(new AppConfigProviderSpec(clientId, applicationName, environmentName, $"{environmentName}.RSC.RWA.Config"));

                while (!cancellationToken.IsCancellationRequested)
                {
                    await rscCfgProvider.GetConfig();
                    await rscGgCfgProvider.GetConfig();
                    await rscRwaCfgProvider.GetConfig();

                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                }

            }, cancellationToken);
        }

        static StartArguments ParseArguments(string[] args)
        {
            var result = new StartArguments();

            foreach (var arg in args)
            {
                var split = arg.Split("=");

                switch (split[0])
                {
                    case "-app":
                        result.Application = split[1];
                        break;

                    case "-env":
                        result.Environment = split[1];
                        break;

                    case "-cc":
                        result.ClientsCount = int.Parse(split[1]);
                        break;
                }
            }

            return result;
        }
    }
}
