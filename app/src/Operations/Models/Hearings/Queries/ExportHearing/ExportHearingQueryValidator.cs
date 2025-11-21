using Agora.Models.Enums;
using FluentValidation;

namespace Agora.Operations.Models.Hearings.Queries.ExportHearing
{
    public class ExportHearingQueryValidator : AbstractValidator<ExportHearingQuery>
    {
        public ExportHearingQueryValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
            RuleFor(c => c.Format).IsInEnum().Must(x => x != ExportFormat.NONE);
        }
    }
}