using BallerupKommune.DAOs.Messages.EBoks;
using BallerupKommune.Operations.Common.Interfaces;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Messages
{
    public class EBoksService : IEBoksService
    {
        private readonly EBoksClient _eBoksClient;

        public EBoksService(EBoksClient eBoksClient)
        {
            _eBoksClient = eBoksClient;
        }

        public async Task<bool> SendMessage(string subject, string content, string recipient)
        {
            return await _eBoksClient.Send(subject, recipient, subject, content);
        }
    }
}