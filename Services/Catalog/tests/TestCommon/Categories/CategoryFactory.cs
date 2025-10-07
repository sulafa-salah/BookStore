using Catalog.Domain.CategoryAggreate;
using TestCommon.TestConstants;

namespace TestCommon.Categories;
    public static class CategoryFactory
    {
    public static Category Create(
        string name = Constants.Category.Name,
        string description = Constants.Category.Description,
        bool isActive = true, Guid? id = null)
    {
       return  new(name, description, isActive, id: id ?? Guid.NewGuid());
    }
    }