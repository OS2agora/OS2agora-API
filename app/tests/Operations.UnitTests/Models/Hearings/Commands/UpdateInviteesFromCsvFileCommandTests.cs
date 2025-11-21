using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromCsvFile;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands;

public class UpdateInviteesFromCsvFileCommandTests : ModelsTestBase<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>
{
    public UpdateInviteesFromCsvFileCommandTests()
    {
        RequestHandlerDelegateMock.Setup(x => x())
            .Returns(Task.FromResult(new MetaDataResponse<Hearing, InvitationMetaData>()));
    }

    [Test]
    public async Task UpdateInviteesFromCsvFile_HearingOwner_Should_Not_Throw_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
            .Returns(true);

        var request = new UpdateInviteesFromCsvFileCommand();

        var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

        Assert.IsNotNull(result);
    }

    [Test]
    public void UpdateInviteesFromCsvFile_NotHearingOwner_Throws_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
            .Returns(false);

        var request = new UpdateInviteesFromCsvFileCommand();

        FluentActions
            .Invoking(() =>
                SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
            .Should().Throw<ForbiddenAccessException>();
    }
}