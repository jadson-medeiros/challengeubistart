using FluentValidation;
using System;

namespace ChallengeUbistart.Business.Models.Validations
{
    public class ItemValidation  : AbstractValidator<Item>
    {
        public ItemValidation()
        {
            RuleFor(f => f.Description)
                .NotEmpty().WithMessage("The {PropertyName} is required.");

            RuleFor(f => f.FinishedAt < DateTime.Now)
                .Equal(true).WithMessage("The {PropertyName} is required.");
        }
    }
}