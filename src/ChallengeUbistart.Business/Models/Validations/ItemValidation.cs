using FluentValidation;

namespace ChallengeUbistart.Business.Models.Validations
{
    public class ItemValidation  : AbstractValidator<Item>
    {
        public ItemValidation()
        {
            RuleFor(f => f.Description)
                .NotEmpty().WithMessage("The {PropertyName} is required.");
        }
    }
}