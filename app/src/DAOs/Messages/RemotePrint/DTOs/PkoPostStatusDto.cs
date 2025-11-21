using Newtonsoft.Json;
using System;

namespace Agora.DAOs.Messages.RemotePrint.DTOs
{
    public class PkoPostStatusDto
    {
        [JsonProperty("MessageUUID")]
        public string MessageUUID { get; set; }

        [JsonProperty("MessageId")]
        public string MessageId { get; set; }

        [JsonProperty("KanalKode")]
        public string Channel { get; set; }

        [JsonProperty("TransaktionsDatoTid")]
        public DateTimeOffset TransactionTime { get; set; }

        [JsonProperty("TransaktionsStatusKode")]
        public string TransactionsStatusCode { get; set; }

        [JsonProperty("CorrelationId")]
        public string CorrelationId { get; set; }

        [JsonProperty("FejlDetaljer")]
        public FejlDetaljer Error { get; set; }

        // Digital Post specific entries
        [JsonProperty("TransmissionId")]
        public string TransmissionId { get; set; }

        // Physical Letter specific entries
        [JsonProperty("AfsendelseIdentifikator")]
        public string DispatchIdentifier { get; set; }

        [JsonProperty("ForsendelseIdentifikator")]
        public string ShipmentIdentifier { get; set; }

        [JsonProperty("BrugerNavn")]
        public string UserName { get; set; }

        [JsonProperty("EnhedTekst")]
        public string UnitText { get; set; }

        [JsonProperty("AfsenderSystemIdentifikator")]
        public string SenderSystemIdentifier { get; set; }

        [JsonProperty("ForsendelseTypeIdentifikator")]
        public string ShipmentTypeIdentifier { get; set; }
    }

    public class FejlDetaljer
    {
        [JsonProperty("FejlTekst")]
        public string Message { get; set; }

        [JsonProperty("FejlKode")]
        public string Code { get; set; }
    }
}
