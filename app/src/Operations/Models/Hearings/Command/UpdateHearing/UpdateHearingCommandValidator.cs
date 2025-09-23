using BallerupKommune.Primitives.Constants;
using FluentValidation;

namespace BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing
{
    public class UpdateHearingCommandValidator : AbstractValidator<UpdateHearingCommand>
    {
        
        public UpdateHearingCommandValidator()
        {
            
            RuleFor(c => c.Hearing.Id).NotEqual(0);

            RuleFor(h => h.Hearing.HearingStatusId).NotEmpty();
            RuleFor(h => h.Hearing.HearingTypeId).NotEmpty();
            RuleFor(h => h.Hearing.SubjectAreaId).NotEmpty();
            RuleFor(h => h.Hearing.KleHierarchyId).NotEmpty();

            RuleFor(h => h.Hearing.StartDate).NotNull();
            RuleFor(h => h.Hearing.Deadline).NotNull();
            RuleFor(h => h.Hearing.Deadline).GreaterThan(h => h.Hearing.StartDate);

            RuleFor(h => h.Hearing.ContactPersonDepartmentName).NotEmpty();
            RuleFor(h => h.Hearing.ContactPersonEmail).NotEmpty().Matches(ValidationRegex.EmailRegex);
            RuleFor(h => h.Hearing.ContactPersonPhoneNumber).NotEmpty().Matches(ValidationRegex.PhoneNumberRegex);
            RuleFor(h => h.Hearing.EsdhTitle).NotEmpty();
        }
    }
}