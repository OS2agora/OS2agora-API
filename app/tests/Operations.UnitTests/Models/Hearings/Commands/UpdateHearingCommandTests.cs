using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.Hearings.Commands
{
    public class UpdateHearingCommandTests : ModelsTestBase<UpdateHearingCommand, Hearing>
    {
        public UpdateHearingCommandTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdateHearingCommand_HasRole(bool shouldSucceed)
        {
            var request = new UpdateHearingCommand
            {
                Hearing = GetHandlerResults()
            };

            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(shouldSucceed);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(shouldSucceed);

            if (!shouldSucceed)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsTrue(result.HearingType != null && result.SubjectArea != null && result.KleHierarchy != null &&
                              result.ClosedHearing && result.Comments.Any() &&
                              !string.IsNullOrEmpty(result.ContactPersonDepartmentName)
                              && !string.IsNullOrEmpty(result.ContactPersonEmail) && !string.IsNullOrEmpty(result.ContactPersonName) &&
                              !string.IsNullOrEmpty(result.ContactPersonPhoneNumber) && result.Contents.Any()
                              && result.Deadline != null && result.HearingStatus != null && result.StartDate != null &&
                              result.Notifications.Any() && result.ShowComments && result.UserHearingRoles.Any());
            }
        }

        private Hearing GetHandlerResults()
        {
            return new Hearing()
            {
                Id = 2,
                HearingType = new HearingType(),
                SubjectArea = new SubjectArea(),
                KleHierarchy = new KleHierarchy(),
                ClosedHearing = true,
                Comments = new List<Comment>
                {
                    new Comment()
                },
                ContactPersonDepartmentName = "Example",
                ContactPersonEmail = "Example@example.com",
                ContactPersonName = "Example",
                ContactPersonPhoneNumber = "99999999",
                Contents = new List<Content>
                {
                    new Content()
                },
                Deadline = DateTime.MaxValue,
                HearingStatus = new HearingStatus(),
                StartDate = DateTime.MinValue,
                Notifications = new List<Notification>
                {
                    new Notification()
                },
                ShowComments = true,
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole()
                }
            };
        } 
    }
}