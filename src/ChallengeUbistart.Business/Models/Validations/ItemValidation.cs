using FluentValidation;
using System;

namespace ChallengeUbistart.Business.Models.Validations
{
    public class ItemValidation  : AbstractValidator<Item>
    {
        public ItemValidation()
        {
            RuleFor(f => f.Description)
                .NotEmpty().WithMessage("The Description is required.")
                .Length(2, 1000).WithMessage("The field Description need to have between 2 and 1000 characters");
            
            RuleFor(f => f.ItemStatus != ItemStatus.Finished)
                .Equal(true).WithMessage("This Item was finished. So CANNOT be edited.");

            RuleFor(f => f.DueDate != DateTime.MinValue)
                .Equal(true).WithMessage("The Due Date is required.");
        }
    }
}