using System;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class FacetDto
    {
        public int ID { get; set; }
        public int FacetTypeID { get; set; }
        public string Nummer { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
        public int BevaringID { get; set; }
        public DateTimeOffset Oprettet { get; set; }
        public DateTimeOffset Rettet { get; set; }
        public DateTimeOffset Udgaaet { get; set; }
    }
}