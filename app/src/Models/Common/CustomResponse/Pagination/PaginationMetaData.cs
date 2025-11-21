namespace Agora.Models.Common.CustomResponse.Pagination
{
    public class PaginationMetaData
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
