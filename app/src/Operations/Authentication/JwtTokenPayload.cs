namespace Agora.Operations.Authentication
{
    public class JwtTokenPayload
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string ApplicationUserId { get; set; }
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHearingOwner { get; set; }
        public string AuthenticationMethod { get; set; }
        public bool IsUser => true;
    }
}