using FluentValidation;

namespace Agora.Operations.Models.CityAreas.Command.UpdateCityArea
{
    public class UpdateCityAreaCommandValidator : AbstractValidator<UpdateCityAreaCommand>
    {
        public UpdateCityAreaCommandValidator()
        {
            RuleFor(c => c.CityArea.Id).NotEqual(0);
            RuleFor(c => c.CityArea.Name).Must(n => n != string.Empty);
        }
    }
}