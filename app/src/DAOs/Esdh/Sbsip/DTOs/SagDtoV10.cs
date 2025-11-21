using System;
using System.Collections.Generic;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class SagDtoV10
    {
        public ICollection<SagsTypeDtoV10> SagsTyper { get; set; }
        public string SagsTitel { get; set; }
        public ICollection<PersonDto> Personer { get; set; }
        public ICollection<FirmaDto> Firmaer { get; set; }
        public PartDto PrimaryPart { get; set; }
        public int Id { get; set; }
        public Guid SagIdentity { get; set; }
        public DateTimeOffset Opstaaet { get; set; }
        public string Nummer { get; set; }
        public FagomraadeDto Fagomraade { get; set; }
        public string Kommentar { get; set; }
        public int BevaringId { get; set; }
        public int KommuneId { get; set; }
        public int KommuneFoer2007Id { get; set; }
        public bool ErSamlesag { get; set; }
        public string SenesteStatusAendringKommentar { get; set; }
        public DateTimeOffset SenesteStatusAendring { get; set; }
        public DateTimeOffset KassationsDato { get; set; }
        public int AmtId { get; set; }
        public int RegionId { get; set; }
        public bool YderligereMaterialeFindes { get; set; }
        public string YderligereMaterialeBeskrivelse { get; set; }
        public string ArkivNote { get; set; }
        public DateTimeOffset Oprettet { get; set; }
        public DateTimeOffset SenestAendret { get; set; }
        public bool ErBeskyttet { get; set; }
        public bool ErBesluttet { get; set; }
        public DateTimeOffset Besluttet { get; set; }
        public string BeslutningNotat { get; set; }
        public DateTimeOffset BeslutningDeadline { get; set; }
        public bool BeslutningHarDeadline { get; set; }
        public DateTimeOffset SletningsDato { get; set; }
        public int SagsNummerId { get; set; }
        public BaseUuidDto Ansaettelsessted { get; set; }
        public BrugerDto Behandler { get; set; }
        public SagsStatusDto SagsStatus { get; set; }
        public int ArkivAfklaringStatusId { get; set; }
        public BrugerDto OprettetAf { get; set; }
        public BrugerDto SenestAendretAf { get; set; }
        public BaseDto StyringsreolHylde { get; set; }
        public int SecuritySetId { get; set; }
        public int BeslutningsTypeId { get; set; }
        public BaseUuidDto SagSkabelon { get; set; }
        public SagNummerDto SagNummer { get; set; }
    }
}