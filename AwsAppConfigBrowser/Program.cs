using System;
using System.Threading.Tasks;
using AwsAppConfigBrowser.Commands;

namespace AwsAppConfigBrowser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var commandFactory = new CommandFactory();
            Console.WriteLine("Enter commands");
            while (true)
            {
                try
                {
                    var promptedCommand = Console.ReadLine();
                    var command = commandFactory.Create(promptedCommand);
                    var exit = await command.Run();
                    if (exit) break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
