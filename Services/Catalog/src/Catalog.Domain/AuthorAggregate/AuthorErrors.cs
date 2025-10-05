using ErrorOr;


namespace Catalog.Domain.AuthorAggregate;
    public static class AuthorErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            code: "Author.NotFound",
            description: "Author not found.");

        public static readonly Error DuplicateName = Error.Conflict(
            code: "Author.DuplicateName",
            description: "An author with the same name already exists.");

        public static readonly Error InvalidName = Error.Validation(
            code: "Author.InvalidName",
            description: "Author name cannot be empty.");

        public static readonly Error InvalidBiography = Error.Validation(
            code: "Author.InvalidBiography",
            description: "Author biography cannot be empty.");
    }