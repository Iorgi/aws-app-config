using System;
using System.Collections.Generic;
using Amazon.AppConfig;
using AwsAppConfig.Common.Commands;
using AwsAppConfigDeployment;

namespace AwsAppConfigBrowser.Commands
{
    public class CommandFactory
    {
        private static readonly AmazonAppConfigClient AmazonAppConfigClient = new AmazonAppConfigClient(Secrets.AwsKey, Secrets.AwsSecret);

        private readonly Dictionary<string, ICommand> _commandsMapping;

        public CommandFactory()
        {
            _commandsMapping = new Dictionary<string, ICommand>
            {
                ["exit"] = new ExitCommand(),
                ["list-apps"] = new ListApplicationsCommand(AmazonAppConfigClient),
                ["list-envs"] = new ListEnvironmentsCommand(AmazonAppConfigClient),
            };

            _commandsMapping.Add("help", new HelpCommand(_commandsMapping.Keys));
        }

        public ICommand Create(string commandName)
        {
            if (_commandsMapping.ContainsKey(commandName))
                return _commandsMapping[commandName];

            throw new ApplicationException($"[{commandName}] command not defined");
        }
    }
}