namespace Agora.Operations.ApplicationOptions
{
    public class EmailOptions
    {
        public static string Email = "Email";

        public string DefaultFromEmail { get; set; }
        public string DefaultFromName { get; set; }
        public string SmtpHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        public int SmtpPort { get; set; }

        public bool UseCredentials { get; set; }

        public bool UseMsGraph { get; set; }
        public string MsGraphMailAddress { get; set; }
    }
}