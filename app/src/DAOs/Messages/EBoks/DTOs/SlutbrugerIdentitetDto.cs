namespace BallerupKommune.DAOs.Messages.EBoks.DTOs
{
    internal class SlutbrugerIdentitetDto
    {
        // 10 numbers - No dash
        public string CprNummerIdentifikator { get; set; }

        // 8 numbers
        public string CvrNummerIdentifikator { get; set; }
    }
}