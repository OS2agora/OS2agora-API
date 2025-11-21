using System.Collections.Generic;

namespace Agora.DAOs.Messages.RemotePrint.DTOs
{
    public class LetterDto
    {
        public string Identifier { get; set; }
        public string Subject { get; set; }
        public string PdfFileBase64 { get; set; }
        public string Name { get; set; }
        public AddressInformationDto AddressInformation { get; set; }
        public bool? CanReceiveDigitalMail { get; set; } = null;
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
    }
}