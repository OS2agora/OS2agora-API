using Agora.Operations.Common.Messages;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces
{
    public interface IEmailService
    {
        Task<NotificationSentReceipt> SendMessage(string subject, string content, string recipient);
    }
}