namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface ICookieService
    {
        void SetAccessCookieInResponse(string value, string apiKey);
        void RemoveAccessCookieInResponse(string apiKey);
        string ReadAccessCookieFromRequest(string apiKey);
    }
}