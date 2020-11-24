using System.Threading.Tasks;

namespace AwsAppConfig.Common.Commands
{
    public interface ICommand
    {
        Task<bool> Run();
    }
}