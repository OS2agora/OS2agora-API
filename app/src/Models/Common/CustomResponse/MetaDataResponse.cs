namespace Agora.Models.Common.CustomResponse
{
    public class MetaDataResponse<TModel, TMeta>
    {
        public TModel ResponseData { get; set; }
        public TMeta Meta { get; set; }
    }
}