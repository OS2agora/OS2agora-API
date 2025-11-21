using System;
using System.Collections.Generic;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class DokumentMetadataDtoV10
    {
        public string EksternId { get; set; }
        public int SagID { get; set; }
        public int DokumentID { get; set; }
        public int RegistreretAfID { get; set; }
        public DateTimeOffset RegistreringsDato { get; set; }
        public int SecuritySetID { get; set; }
        public Guid DokumentIdentity { get; set; }
        public Guid DokumentRegistreringIdentity { get; set; }
        public string Beskrivelse { get; set; }
        public string DokumentNavn { get; set; }
        public PartDto SagsPart { get; set; }
        public BrugerDto Behandler { get; set; }
        public string SagsNummer { get; set; }
        public bool PaaPostliste { get; set; }
        public string PostlisteTitel { get; set; }
        public string PostlisteBeskrivelse { get; set; }
        public DokumentArtDto DokumentArt { get; set; }
        public List<FilDtoV9> Filer { get; set; }
        public MailDto Mail { get; set; }
        public bool OmfattetAfAktindsigt { get; set; }
        public string AktindsigtKommentar { get; set; }
        public DokumentMetadataDtoV10DeletedState DeletedState { get; set; }
        public DateTimeOffset DeletedDate { get; set; }
        public int DeletedById { get; set; }
        public string DeletedReason { get; set; }
        public DateTimeOffset DeleteConfirmed { get; set; }
        public int DeleteConfirmedById { get; set; }
        public int Id { get; set; }
        public string Navn { get; set; }
    }
}