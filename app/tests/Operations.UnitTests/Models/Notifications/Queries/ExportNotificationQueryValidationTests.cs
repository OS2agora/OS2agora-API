using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Models.Notifications.Queries.ExportNotification;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.UnitTests.Models.Notifications.Queries;

public class ExportNotificationQueryValidationTests
{
    private readonly ExportNotificationQueryValidator _validator = new();

    [Test]
    public async Task ExportNotification_ValidRequest_ShouldNotThrowException()
    {
        var command = new ExportNotificationQuery
        {
            HearingId = 1,
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        await ValidationTestFramework.For<ExportNotificationQuery, FileDownload>()
            .WithValidators(_validator)
            .ShouldPassValidation(command);
    }

    [Test]
    public async Task ExportNotification_EmptyHearingId_ShouldThrowValidationException()
    {
        var command = new ExportNotificationQuery
        {
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        await ValidationTestFramework
            .For<ExportNotificationQuery, FileDownload>()
            .WithValidators(_validator)
            .ShouldFailValidationWithError(command, nameof(ExportNotificationQuery.HearingId));
    }

    [Test]
    public async Task ExportNotification_EmptyNotificationType_ShouldThrowValidationException()
    {
        var command = new ExportNotificationQuery
        {
            HearingId = 1
        };

        await ValidationTestFramework
            .For<ExportNotificationQuery, FileDownload>()
            .WithValidators(_validator)
            .ShouldFailValidationWithError(command, nameof(ExportNotificationQuery.NotificationType));
    }
}