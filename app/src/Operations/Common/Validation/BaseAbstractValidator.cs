using Agora.Primitives.Logic;
using FluentValidation;

namespace Agora.Operations.Common.Validation
{
    public abstract class BaseAbstractValidator<T> : AbstractValidator<T> where T : class
    {

        protected void AddMunicipalitySpecificValidators()
        {
            switch (true)
            {
                case var _ when MunicipalityProfile.IsCopenhagenMunicipalityProfile():
                    RegisterCopenhagenValidators();
                    break;
                case var _ when MunicipalityProfile.IsBallerupMunicipalityProfile():
                    RegisterBallerupValidators();
                    break;
                case var _ when MunicipalityProfile.IsNovatarisMunicipalityProfile():
                    RegisterNovatarisValidators();
                    break;
                case var _ when MunicipalityProfile.IsOS2MunicipalityProfile():
                    RegisterOS2Validators();
                    break;
            }
        }

        protected abstract void RegisterCopenhagenValidators();
        protected abstract void RegisterBallerupValidators();
        protected abstract void RegisterNovatarisValidators();
        protected abstract void RegisterOS2Validators();
    }
}