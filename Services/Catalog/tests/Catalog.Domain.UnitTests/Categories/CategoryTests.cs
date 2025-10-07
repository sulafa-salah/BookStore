using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon.Categories;

namespace Catalog.Domain.UnitTests.Categories;
    public class CategoryTests
    {
    [Fact]
    public void CreateCategory_WhenValid_ShouldSetFields()
    {
        // arrange + act
        var category = CategoryFactory.Create();

        // assert
        category.Name.Should().Be("Fiction");
        category.Description.Should().Be("All fiction");
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateCategory_WhenValid_ShouldSetFields()
    {
        // arrange
        var category = CategoryFactory.Create("Fiction", "All fiction", true);

        // act
        category.Update("Technology", "Programming & AI", false);

        // assert
        category.Name.Should().Be("Technology");
        category.Description.Should().Be("Programming & AI");
        category.IsActive.Should().BeFalse();
    }


}

