using System;
using System.Collections.Generic;

namespace Agora.DAOs.Esdh.Sbsip.DTOs
{
    public class MailDto
    {
        public string Subject { get; set; }
        public string FromEmailAddress { get; set; }
        public string FromNavn { get; set; }
        public List<MailRecipientDto> MailRecipients { get; set; }
        public DateTimeOffset SentDate { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public bool IsSentItem { get; set; }
        public string AccountID { get; set; }
        public string NativeID { get; set; }
        public MailDtoMailType MailType { get; set; }
    }
}