namespace Agora.Api.Services.Interfaces
{
    public interface IClaimsService<T>
    {
        string GetNameIdentifier();
        string GetFullName();
        string GetName();
        string GetFirstName();
        string GetLastName();
        string GetDisplayName();
        string GetEmail();
        string GetOrganizationName();

        string GetPidNumberIdentifier();
        string GetCprNumberIdentifier();
        string GetCvrNumberIdentifier();
        string GetRidNumberIdentifier();
        string GetCompanyNameIdentifier();

        string GetRoles();

        string GetSubject();
        string GetUniqueIdentifier();
        string GetTokenUsage();
        string GetJwtId();
        string GetScope();
        string GetAudience();
        string GetAzp();
        string GetIssuedAt();
        string GetNotBefore();
        string GetExpirationTime();
        string GetIssuer();
    }
}