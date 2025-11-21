using Agora.Models.Models;
using Agora.Operations.Models.NotificationContentSpecifications.Queries;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.NotificationContentSpecifications.Queries
{
    public class GetNotificationContentSpecificationsValidationTests
    {
        private readonly GetNotificationContentSpecificationsQueryValidator _validator = new();

        [Test]
        public async Task GetNotificationContentSpecifications_ValidRequest_ShouldNotThrowException()
        {
            var command = new GetNotificationContentSpecificationsQuery
            {
                HearingId = 1,
            };
            await ValidationTestFramework
                .For<GetNotificationContentSpecificationsQuery, List<NotificationContentSpecification>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task GetNotificationContentSpecifications_EmptyHearingTypeId_ShouldThrowValidationException()
        {
            var command = new GetNotificationContentSpecificationsQuery();

            await ValidationTestFramework
                .For<GetNotificationContentSpecificationsQuery, List<NotificationContentSpecification>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(GetNotificationContentSpecificationsQuery.HearingId));
        }
    }
}