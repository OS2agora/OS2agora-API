namespace BallerupKommune.DAOs.Esdh.Sbsip.DTOs
{
    public class SagsStatusDto
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public int Orden { get; set; }
        public SagsStatusDtoSagsTilstand SagsTilstand { get; set; }
        public bool RequireComments { get; set; }
        public string SagsStatusKommentar { get; set; }
    }
}