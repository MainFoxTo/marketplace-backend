using FluentValidation.TestHelper;
using Inventory.Api.Validators;
using Inventory.Application.DTOs;
using System;
using Xunit;

namespace Inventory.Tests.Unit.Validators
{
    public class ReserveRequestValidatorTests
    {
        private readonly ReserveRequestValidator _validator = new();

        [Fact]
        public void Should_HaveError_When_ProductIdIsEmpty()
        {
            var dto = new ReserveRequestDto { ProductId = Guid.Empty, Quantity = 5, CorrelationId = Guid.NewGuid() };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public void Should_HaveError_When_QuantityIsZero()
        {
            var dto = new ReserveRequestDto { ProductId = Guid.NewGuid(), Quantity = 0, CorrelationId = Guid.NewGuid() };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_HaveError_When_CorrelationIdIsEmpty()
        {
            var dto = new ReserveRequestDto { ProductId = Guid.NewGuid(), Quantity = 5, CorrelationId = Guid.Empty };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.CorrelationId);
        }

        [Fact]
        public void Should_PassValidation_When_AllFieldsValid()
        {
            var dto = new ReserveRequestDto { ProductId = Guid.NewGuid(), Quantity = 5, CorrelationId = Guid.NewGuid() };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
