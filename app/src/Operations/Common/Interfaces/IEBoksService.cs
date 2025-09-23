using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IEBoksService
    {
        Task<bool> SendMessage(string subject, string content, string recipient);
    }
}