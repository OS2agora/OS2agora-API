namespace Agora.Operations.Common.Constants
{
    public static class JWT
    {
        public static class Roles
        {
            public const string Admin = "Administrator";
            public const string HearingCreator = "HearingCreator";
        }

        public static class Claims
        {
            public const string Name = "name";
            public const string DatabaseUserId = "databaseUserId";
            public const string EmployeeDisplayName = "employeeDisplayName";
            public const string AuthenticationMethod = "authenticationMethod";
            public const string CompanyId = "companyId";
            public const string RefreshToken = "refreshToken";
            public const string MainSessionExpiration = "mainSessionExpiration";
        }

        public static class Cookie
        {
            public const string AccessCookieName = "hp_access_cookie";
        }
    }
}