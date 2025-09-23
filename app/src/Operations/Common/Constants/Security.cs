namespace BallerupKommune.Operations.Common.Constants
{
    public static class Security
    {
        public static class Roles
        {
            // Idp roles
            public const string Administrator = "Administrator";
            public const string HearingCreator = "HearingCreator";
            
            // Hearing contextual roles
            public const string HearingOwner = "HearingOwner";
            public const string HearingResponder = "HearingResponder";
            public const string HearingInvitee = "HearingInvitee";
            public const string HearingReviewer = "HearingReviewer";

            // Login contextual roles
            public const string Anonymous = "Anonymous";
            public const string Citizen = "Citizen";
            public const string Employee = "Employee";
        }
    }
}
