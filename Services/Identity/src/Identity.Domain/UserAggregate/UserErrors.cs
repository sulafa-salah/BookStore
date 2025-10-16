using ErrorOr;

namespace Identity.Domain.UserAggregate;
    public static class UserErrors
    {
        public static readonly Error InvalidFirstName =
            Error.Validation("User.InvalidFirstName", "First name is required.");

        public static readonly Error InvalidLastName =
            Error.Validation("User.InvalidLastName", "Last name is required.");

        public static readonly Error InvalidEmail =
            Error.Validation("User.InvalidEmail", "Email is required.");

        public static readonly Error InvalidEmailFormat =
            Error.Validation("User.InvalidEmailFormat", "Email format is invalid.");
    public static readonly Error EmailAlreadyUsed = 
        Error.Conflict("User.EmailAlreadyUsed", "Email is already registered.");

    public static readonly Error AlreadyExists =
            Error.Conflict("User.AlreadyExists", "A user with this email already exists.");

        public static readonly Error Inactive =
            Error.Validation("User.Inactive", "User account is inactive.");

        public static readonly Error NotFound =
            Error.NotFound("User.NotFound", "User not found.");
    }