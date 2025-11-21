using Agora.DTOs.Common;
using Agora.DTOs.Enums;
using System;

namespace Agora.DTOs.Models
{
    public class NotificationQueueDto : AuditableDto<NotificationQueueDto>
    {
        public bool IsSent { get; set; }
        public DateTime? SuccessfulSentDate { get; set; }
        public DateTime? SuccessfulDeliveryDate { get; set; }
        public NotificationDeliveryStatus DeliveryStatus { get; set; }
        public NotificationSentAs SentAs { get; set; }
    }
}