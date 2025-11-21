using FluentValidation;

namespace Agora.Operations.Models.CityAreas.Command.DeleteCityArea
{
    public class DeleteCityAreaCommandValidator : AbstractValidator<DeleteCityAreaCommand>
    {
        public DeleteCityAreaCommandValidator()
        {
            RuleFor(c => c.Id).NotEqual(0);
        }
    }
}