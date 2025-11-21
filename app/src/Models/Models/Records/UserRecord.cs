namespace Agora.Models.Models.Records
{
    public class UserRecord
    {
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StreetName { get; set; }

        public string Cpr { get; set; }
        public string Email { get; set; }

        public bool IsInvitee { get; set; }
        public bool IsResponder { get; set; }
        public bool IsCompany { get; set; }

        public CompanyRecord Company { get; set; }
    }
}