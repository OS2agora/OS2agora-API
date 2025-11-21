using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Models.Multiparts;

namespace Agora.Operations.Common.Interfaces
{
    public interface IFieldsValidator
    {
        Task ValidateMultipartFields(int hearingId, List<MultiPartField> multiPartFields);
    }
}