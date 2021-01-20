using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Business.Notifications;
using FluentValidation;
using FluentValidation.Results;

namespace ChallengeUbistart.Business.Services
{
    public abstract class BaseService
    {
        private readonly INotify _notify;

        protected BaseService(INotify notify)
        {
            _notify = notify;
        }

        protected void Inform(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Inform(error.ErrorMessage);
            }
        }

        protected void Inform(string message)
        {
            _notify.Handle(new Notification(message));
        }

        protected bool ExecuteValidation<TV, TE>(TV validation, TE entity) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validation.Validate(entity);

            if (validator.IsValid) return true;

            Inform(validator);

            return false;
        }
    }
}