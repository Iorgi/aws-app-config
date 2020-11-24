using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwsAppConfig.Common.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IEnumerable<string> _commands;

        public HelpCommand(IEnumerable<string> commands)
        {
            _commands = commands;
        }

        public Task<bool> Run()
        {
            foreach (var command in _commands)
                Console.WriteLine(command);

            return Task.FromResult(false);
        }
    }
}