using Agora.Entities.Entities;
using Agora.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Persistence.DefaultData.Ballerup
{
    public static class BallerupNotificationTemplates
    {
        public static async Task<List<NotificationTemplateEntity>> GetEntities(ApplicationDbContext context)
        {
            var notificationContentTypes = await context.NotificationContentTypes.ToListAsync();
            var headerNotificationContentTypeId = notificationContentTypes.SingleOrDefault(nct => nct.Type == NotificationContentType.HEADER)!.Id;

            return new List<NotificationTemplateEntity>
            {
                new NotificationTemplateEntity {
                    Name = "Default høring konkluderet header",
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Der foreligger nu en konklusion på høringen: {{HearingTitle}}, som du har svaret på.{{NewLine}}{{NewLine}}Du kan se konklusionen på høringen her: {{LinkToHearing}}"
                },
                new NotificationTemplateEntity {
                    Name = "Default høring opdateret header",
                    NotificationContentTypeId = headerNotificationContentTypeId,
                    TextContent = "Der er foretaget en ændring på høringen '{{HearingTitle}}', som du har svaret på.{{NewLine}}{{NewLine}}Der er tale om en mindre rettelse, men dette kan stadig have indflydelse på dit afgivne svar.{{NewLine}} Du kan se den opdaterede høringstekst samt rette dit svar her: {{LinkToHearing}}"
                },
            };
        }
    }
}
