namespace Todo.Api.Domain.IAM.Constants;

public static class IAMConstant
{
    public static class Constraints
    {
        public const int EmailMaxLength = 256;
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;
    }

    public static class Defaults
    {
        public const int AccessTokenMinutes = 15;
        public const int RefreshTokenDays = 7;
    }

    public static class Claims
    {
        public const string UserId = "sub";
        public const string Email = "email";
        public const string Name = "name";
        public const string JwtId = "jti";
    }

    public static class Token
    {
        public const string Bearer = "Bearer";
    }
}