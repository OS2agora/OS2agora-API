using BallerupKommune.Models.Models.Esdh;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IEsdhService
    {
        Case DeserializeMetaDataFromHearing(Hearing hearing);
        Task<Case> CreateCase(int hearingId, string esdhTitle, int kleHierarchyId);
        Task<Case> ChangeHearingOwner(int hearingId, int userId);
        Task JournalizeHearingAnswer(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType);
        Task JournalizeHearingText(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType);
        Task JournalizeHearingConclusion(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType);
        Task<Case> CloseCase(int hearingId, string comment = null);
    }
}