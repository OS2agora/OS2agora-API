namespace Agora.Models.Common.CustomResponse.SortAndFilter
{
    public class SortingParameters
    {
        public Sorting Sorting { get; set; } = null;
        public bool hasSorting => Sorting != null;
    }
}
