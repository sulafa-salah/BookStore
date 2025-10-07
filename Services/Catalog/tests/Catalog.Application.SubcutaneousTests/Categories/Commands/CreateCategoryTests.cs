using Catalog.Domain.CategoryAggreate;
using FluentAssertions;
using Catalog.Application.SubcutaneousTests.Common;
using MediatR;

using TestCommon.Categories;

namespace Catalog.Application.SubcutaneousTests.Categories.Commands;




[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateCategoryTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();

    [Fact]
    public async Task CreateCategory_WhenValidCommand_ShouldCreateCategory()
    {
        // Arrange
        var category = await CreateCategory();

        // Create a valid CreateCategoryCommand
        var createCategoryCommand = CategoryCommandFactory.CreateCreateCategoryCommand(name: "Technology",
            description: "Programming & AI");

        // Act
        var createCategoryResult = await _mediator.Send(createCategoryCommand);

        // Assert

        var created = createCategoryResult.Value;
        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("Technology");
        created.Description.Should().Be("Programming & AI");
        created.IsActive.Should().BeTrue(); // assumes default=true in aggregate ctor
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(200)]
    public async Task CreateCategory_WhenCommandContainsInvalidData_ShouldReturnValidationError(int CategoryNameLength)
    {
        // Arrange
        string categoryName = new('a', CategoryNameLength);
        var createCategoryCommand = CategoryCommandFactory.CreateCreateCategoryCommand(name: categoryName);

        // Act
        var result = await _mediator.Send(createCategoryCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Name");
    }

    [Fact]
    public async Task CreateCategory_WhenNameAlreadyExists_ShouldReturnDuplicateNameError()
    {
        // Arrange – seed one
        var first = await _mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(name: "Fiction"));
        first.IsError.Should().BeFalse();

        // Act – try to create with same name
        var second = await _mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(name: "Fiction"));

        // Assert
        second.IsError.Should().BeTrue();
       
        second.FirstError.Code.Should().Be(CategoryErrors.DuplicateName.Code);
       
    }


    private async Task<Category> CreateCategory()
    {
        //  1. Create a CreateCategoryCommand
        var createCategoryCommand = CategoryCommandFactory.CreateCreateCategoryCommand();

        //  2. Sending it to MediatR
        var result = await _mediator.Send(createCategoryCommand);

        //  3. Making sure it was created successfully
        result.IsError.Should().BeFalse();
        return result.Value;
    }
}