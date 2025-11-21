namespace Agora.Models.Common.CustomResponse.SortAndFilter
{
    public class Filter
    {
        public string Property { get; set; }
        public string Value { get; set; }
        public string Operation { get; set; } = "equals";
    }
}
