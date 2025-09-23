using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMessage(string subject, string content, string recipient);
    }
}