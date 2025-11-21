using Agora.Operations.Common.Validation;
using Agora.Primitives.Constants;
using FluentValidation;

namespace Agora.Operations.Models.Hearings.Command.UpdateHearing
{
    public class UpdateHearingCommandValidator : BaseAbstractValidator<UpdateHearingCommand>
    {
        
        public UpdateHearingCommandValidator()
        {
            // Common validation rules
            RuleFor(c => c.Hearing.Id).NotEqual(0);

            RuleFor(h => h.Hearing.HearingStatusId).NotEmpty();
            RuleFor(h => h.Hearing.HearingTypeId).NotEmpty();
            RuleFor(h => h.Hearing.SubjectAreaId).NotEmpty();
            RuleFor(h => h.Hearing.KleHierarchyId).NotEmpty();

            RuleFor(h => h.Hearing.StartDate).NotNull();
            RuleFor(h => h.Hearing.Deadline).NotNull();
            RuleFor(h => h.Hearing.Deadline).GreaterThan(h => h.Hearing.StartDate);

            RuleFor(h => h.Hearing.EsdhTitle).NotEmpty();
            RuleFor(h => h.Hearing.ContactPersonDepartmentName).NotEmpty();

            AddMunicipalitySpecificValidators();
        }

        protected override void RegisterCopenhagenValidators()
        {
            RuleFor(h => h.Hearing.ContactPersonEmail).NotEmpty().Matches(ValidationRegex.LinkRegex);
            RuleFor(h => h.Hearing.ContactPersonPhoneNumber).Matches(ValidationRegex.PhoneNumberRegex)
                .When(h => !string.IsNullOrEmpty(h.Hearing.ContactPersonPhoneNumber));
            RuleFor(h => h.Hearing.CityAreaId).NotEmpty();
        }

        protected override void RegisterBallerupValidators()
        {
            RuleFor(h => h.Hearing.ContactPersonEmail).NotEmpty().Matches(ValidationRegex.EmailRegex);
            RuleFor(h => h.Hearing.ContactPersonPhoneNumber).Matches(ValidationRegex.PhoneNumberRegex)
                .When(h => !string.IsNullOrEmpty(h.Hearing.ContactPersonPhoneNumber));
        }

        protected override void RegisterNovatarisValidators()
        {
            RuleFor(h => h.Hearing.ContactPersonEmail).NotEmpty().Matches(ValidationRegex.EmailRegex);
            RuleFor(h => h.Hearing.ContactPersonPhoneNumber).NotEmpty().Matches(ValidationRegex.PhoneNumberRegex);
        }

        protected override void RegisterOS2Validators()
        {
            RuleFor(h => h.Hearing.ContactPersonEmail).NotEmpty().Matches(ValidationRegex.EmailRegex);
            RuleFor(h => h.Hearing.ContactPersonPhoneNumber).NotEmpty().Matches(ValidationRegex.PhoneNumberRegex);
        }

    }
}