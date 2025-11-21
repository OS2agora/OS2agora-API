using FluentValidation;

namespace Agora.Operations.Models.CityAreas.Command.CreateCityArea
{
    public class CreateCityAreaCommandValidator : AbstractValidator<CreateCityAreaCommand>
    {
        public CreateCityAreaCommandValidator()
        {
            RuleFor(c => c.CityArea.Name).NotEmpty();
        }
    }
}