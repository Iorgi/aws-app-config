using System.Threading.Tasks;

namespace AwsAppConfig.Common.Commands
{
    public class ExitCommand : ICommand
    {
        public Task<bool> Run()
        {
            return Task.FromResult(true);
        }
    }
}