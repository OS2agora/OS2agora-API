using System.Threading.Tasks;
using Agora.Models.Common.CustomResponse.Pagination;

namespace Agora.Operations.Common.Interfaces
{
    public interface IPaginationValidator
    {
        bool ValidatePaginationParameters(PaginationParameters paginationParameters);
    }
}
