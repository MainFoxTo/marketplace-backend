using FluentValidation;
using Inventory.Application.DTOs;

namespace Inventory.Api.Validators
{
    public class ReserveRequestValidator : AbstractValidator<ReserveRequestDto>
    {
        public ReserveRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("productId is required");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("quantity must be > 0");
            RuleFor(x => x.CorrelationId).NotEmpty().WithMessage("correlationId is required");
        }
    }
}
