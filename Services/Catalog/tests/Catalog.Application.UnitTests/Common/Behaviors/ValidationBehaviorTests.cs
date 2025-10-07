using Catalog.Application.Categories.Commands.CreateCategory;
using Catalog.Application.Common.Behaviors;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;

using TestCommon.Categories;


namespace Catalog.Application.UnitTests.Common.Behaviors;

    public class ValidationBehaviorTests
    {
        private readonly ValidationBehavior<CreateCategoryCommand, ErrorOr<Category>> _validationBehavior;
        private readonly IValidator<CreateCategoryCommand> _mockValidator;
        private readonly RequestHandlerDelegate<ErrorOr<Category>> _mockNextBehavior;

        public ValidationBehaviorTests()
        {
        // Create a next behavior (mock) in MediatR pipeline
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<Category>>>();

        // Mock the FluentValidation validator for the CreateCategoryCommand
        _mockValidator = Substitute.For<IValidator<CreateCategoryCommand>>();

      // Create the ValidationBehavior(System Under Test -SUT)
        // This is the real behavior being tested, injected with the mock validator
        _validationBehavior = new ValidationBehavior<CreateCategoryCommand, ErrorOr<Category>>(_mockValidator);
        }

        [Fact]
        public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
        {
        // Arrange
        // Create a sample CreateCategoryCommand and a sample Category result
        var createCategoryRequest = CategoryCommandFactory.CreateCreateCategoryCommand();
            var category = CategoryFactory.Create();

        // Configure the mock validator to return an empty ValidationResult(i.e., no validation errors)
            _mockValidator
                .ValidateAsync(createCategoryRequest, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult());
        // Configure the mock next behavior (the handler) to return a successful Category
        _mockNextBehavior.Invoke().Returns(category);

        // Act
        // Call the Handle method of the validation behavior with the mock request and next delegate
        var result = await _validationBehavior.Handle(createCategoryRequest, _mockNextBehavior, default);

        // Assert
        // Ensure the behavior did not produce an error (validation passed)
        result.IsError.Should().BeFalse();
        // Ensure the result value returned from the behavior equals the category from the next delegate
        result.Value.Should().BeEquivalentTo(category);
        }

        [Fact]
        public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
        {
        // Arrange
        // Create a sample CreateCategoryCommand request
        var createCategoryRequest = CategoryCommandFactory.CreateCreateCategoryCommand();

        // Create a list of validation failures to simulate validation errors
        List<ValidationFailure> validationFailures = [new(propertyName: "Name", errorMessage: "bad Name")];

        // Configure the mock validator to return a ValidationResult containing these errors
        _mockValidator
            .ValidateAsync(createCategoryRequest, Arg.Any<CancellationToken>())
                .Returns(new ValidationResult(validationFailures));

        // Act
        // Execute the validation behavior (the next handler should NOT be called because validation failed)
        var result = await _validationBehavior.Handle(createCategoryRequest, _mockNextBehavior, default);

        // Assert
        // Ensure that the behavior correctly identifies the result as an error
        result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("Name");
            result.FirstError.Description.Should().Be("bad Name");
        }
    }