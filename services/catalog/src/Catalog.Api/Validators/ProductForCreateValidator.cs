using Catalog.Api.DTOs;
using FluentValidation;

namespace Catalog.Api.Validators
{
    public class ProductForCreateValidator : AbstractValidator<ProductForCreateDto>
    {
        public ProductForCreateValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required")
                .MaximumLength(64).WithMessage("SKU cannot exceed 64 characters")
                .Matches("^[a-zA-Z0-9-]+$").WithMessage("SKU can only contain letters, numbers and hyphens");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
