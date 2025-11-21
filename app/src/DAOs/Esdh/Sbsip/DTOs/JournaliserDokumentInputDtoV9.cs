namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    /// <summary>
    /// NOTE: Do not add any extra properties to this class
    /// It will break the multipart-json data when uploading files to SBSYS
    /// </summary>
    public class JournaliserDokumentInputDtoV9
    {
        public int SagID { get; set; }
        public string DokumentNavn { get; set; }
        public string Beskrivelse { get; set; }
    }
}