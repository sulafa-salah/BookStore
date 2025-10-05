using ErrorOr;

namespace Catalog.Domain.CategoryAggreate;
    public static class CategoryErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            code: "Category.NotFound",
            description: "Category not found.");

        public static readonly Error DuplicateName = Error.Conflict(
            code: "Category.DuplicateName",
            description: "A category with the same name already exists.");
    }
