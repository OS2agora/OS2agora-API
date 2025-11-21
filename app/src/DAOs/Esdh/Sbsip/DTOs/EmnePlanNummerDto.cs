using System;
using System.Collections.Generic;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class EmnePlanNummerDto
    {
        public int Id { get; set; }
        public int EmneplanID { get; set; }
        public string Nummer { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
        public int Niveau { get; set; }
        public DateTimeOffset Oprettet { get; set; }
        public DateTimeOffset Rettet { get; set; }
        public DateTimeOffset Udgaaet { get; set; }
        public bool ErUdgaaet { get; set; }
        public ICollection<EmnePlanNummerDto> AfloserNumre { get; set; }
    }
}