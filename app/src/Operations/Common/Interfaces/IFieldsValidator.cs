using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Models.Multiparts;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IFieldsValidator
    {
        Task ValidateMultipartFields(int hearingId, List<MultiPartField> multiPartFields);
    }
}