using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromPersonalInviteeIdentifiers;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands;

public class UpdateInviteesFromPersonalInviteeIdentifiersCommandTests : ModelsTestBase<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>
{
    public UpdateInviteesFromPersonalInviteeIdentifiersCommandTests()
    {
        RequestHandlerDelegateMock.Setup(x => x())
            .Returns(Task.FromResult(new MetaDataResponse<Hearing, InvitationMetaData>()));
    }

    [Test]
    public async Task UpdateInviteesFromPersonalInviteeIdentifiers_HearingOwner_Should_Not_Throw_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
            .Returns(true);

        var request = new UpdateInviteesFromPersonalInviteeIdentifiersCommand();

        var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

        Assert.IsNotNull(result);
    }

    [Test]
    public void UpdateInviteesFromPersonalInviteeIdentifiers_NotHearingOwner_Throws_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
            .Returns(false);

        var request = new UpdateInviteesFromPersonalInviteeIdentifiersCommand();

        FluentActions
            .Invoking(() =>
                SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
            .Should().Throw<ForbiddenAccessException>();
    }
}