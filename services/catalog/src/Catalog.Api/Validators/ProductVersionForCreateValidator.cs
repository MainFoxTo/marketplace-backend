using Catalog.Api.DTOs;
using FluentValidation;

namespace Catalog.Api.Validators
{
    public class ProductVersionForCreateValidator : AbstractValidator<ProductVersionForCreateDto>
    {
        public ProductVersionForCreateValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required")
                .MaximumLength(64).WithMessage("SKU cannot exceed 64 characters")
                .Matches("^[a-zA-Z0-9-]+$").WithMessage("SKU can only contain letters, numbers and hyphens");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Metadata)
                .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Metadata));
        }
    }
}
