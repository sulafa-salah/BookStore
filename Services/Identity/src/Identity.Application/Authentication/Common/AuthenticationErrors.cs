using ErrorOr;


namespace Identity.Application.Authentication.Common;
    public static class AuthenticationErrors
    {
        public static readonly Error InvalidCredentials = Error.Validation(
            code: "Authentication.InvalidCredentials",
            description: "Invalid email or password.");

    public static readonly Error RefreshTokenInvalid = Error.Validation(
          code: "Auth.RefreshTokenInvalid",
            description: "Refresh token is invalid.");
    public static readonly Error RefreshTokenExpired = Error.Validation(
         code: "Auth.RefreshTokenExpired",
            description: "Refresh token expired or revoked.");
    public static readonly Error UserNotFound = Error.NotFound(
        code: "Auth.UserNotFound",
        description: "User not found.");
    public static readonly Error UserDeactivated = Error.Validation(
       code: "Auth.UserDeactivated",
       description: "User not active.");
    



}