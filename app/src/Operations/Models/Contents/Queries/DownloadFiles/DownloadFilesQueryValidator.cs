using FluentValidation;

namespace Agora.Operations.Models.Contents.Queries.DownloadFiles
{
    public class DownloadFilesQueryValidator : AbstractValidator<DownloadFilesQuery>
    {
        public DownloadFilesQueryValidator()
        {
            RuleFor(c => c.HearingId).NotEqual(0);
        }
    }
}