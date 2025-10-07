using Catalog.Application.Categories.Commands.CreateCategory;
using TestCommon.TestConstants;

namespace TestCommon.Categories;


   public static class CategoryCommandFactory
    {
        public static CreateCategoryCommand CreateCreateCategoryCommand(
            string name = Constants.Category.Name,
            string description = Constants.Category.Description)
        {
            return new CreateCategoryCommand(
                Name: name,
               Description: description);
        }
    }