using FluentValidation;

namespace BallerupKommune.Operations.Models.GlobalContents.Queries.GetLatestGlobalContent
{
    public class GetLatestGlobalContentQueryValidator : AbstractValidator<GetLatestGlobalContentQuery>
    {
        public GetLatestGlobalContentQueryValidator()
        {
            RuleFor(t => t.GlobalContentTypeId).NotEmpty();
        }
    }
}